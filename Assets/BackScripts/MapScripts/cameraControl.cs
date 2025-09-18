using UnityEngine;

public class cameraControl : MonoBehaviour
{
    Vector3 touchStart;
    public Camera cam;
    public generateMap map;
    public bool pan = false;
    public float panningSpeed = 1.0f;
    public Vector3 panTo = new Vector3(10, 5, 10);

    public int lowBorderMargin = 1, highBorderMargin = 5;

    [Header("Zoom Settings")]
    public float speed = 0.0046f;
    public float MINSCALE = 2F;
    public float MAXSCALE = 5F;

    private bool wasDragging = false;
    private int prevTouchCount = 0;

    [Header("Rubber Border Settings")]
    public float slowdownStartDistance = 1.5f;
    public float maxSlowdownFactor = 0.3f;
    public float returnSpeedInside = 15f;
    public float returnSpeedOutside = 4.0f;
    public float edgeStiffness = 0.8f;

    // Новые параметры для определения клика
    private float tapStartTime;
    private Vector2 tapStartScreenPos;
    private const float MAX_TAP_DISTANCE_PIXELS = 20f; // Макс смещение пальца, чтобы считать это кликом
    private const float MAX_TAP_DURATION_SECONDS = 0.3f; // Макс длительность тапа

    void Update()
    {
        if (pan)
        {
            panToPosition(panTo);
            return;
        }
        HandleCameraMovement();
    }

    void LateUpdate()
    {
        prevTouchCount = Input.touchCount;
    }

    private void HandleCameraMovement()
    {
        // Двухпальцевый жест — зум + пан
        if (Input.touchCount == 2)
        {
            HandleTwoFingerGestures();
            wasDragging = false;
            return;
        }

        // Однопальцевый жест — пан или клик
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                tapStartTime = Time.time;
                tapStartScreenPos = touch.position;
                touchStart = GetWorldPos(0);
                wasDragging = false; // Пока не знаем — клик это или свайп
            }

            if (touch.phase == TouchPhase.Moved)
            {
                // Если палец сдвинулся слишком сильно — это свайп, не клик
                if (Vector2.Distance(touch.position, tapStartScreenPos) > MAX_TAP_DISTANCE_PIXELS)
                {
                    wasDragging = true;
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                float tapDuration = Time.time - tapStartTime;
                bool isTap = !wasDragging &&
                             tapDuration <= MAX_TAP_DURATION_SECONDS &&
                             Vector2.Distance(touch.position, tapStartScreenPos) <= MAX_TAP_DISTANCE_PIXELS;

                if (isTap)
                {
                    HandleBuildingTap(touch.position);
                }
                wasDragging = false;
            }

            // Обработка свайпа (если это не клик)
            if (wasDragging && touch.phase == TouchPhase.Moved)
            {
                Vector3 direction = touchStart - GetWorldPos(0);
                Vector3 desiredPosition = transform.position + direction;
                transform.position = ApplyRubberBorders(desiredPosition);
                touchStart = GetWorldPos(0);
            }
        }

        // Возвращение в границы, если не тащим
        if (!wasDragging && !checkBorders(transform.position))
        {
            ReturnToBounds();
        }
    }

    private void HandleTwoFingerGestures()
    {
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float currentMagnitude = (touchZero.position - touchOne.position).magnitude;
        float difference = currentMagnitude - prevMagnitude;
        difference /= (Screen.height / 1080f);
        Vector2 currentMidpoint = (touchZero.position + touchOne.position) / 2;
        Vector2 prevMidpoint = (touchZeroPrevPos + touchOnePrevPos) / 2;

        Vector3 worldPosOfPrevMidpoint = GetWorldPos(0, prevMidpoint);
        Vector3 worldPosOfCurrentMidpoint = GetWorldPos(0, currentMidpoint);
        Vector3 panOffset = worldPosOfPrevMidpoint - worldPosOfCurrentMidpoint;

        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - difference * speed, MINSCALE, MAXSCALE);

        Vector3 worldPosAfterZoom = GetWorldPos(0, currentMidpoint);
        Vector3 zoomCenteringOffset = worldPosOfCurrentMidpoint - worldPosAfterZoom;

        transform.position += panOffset + zoomCenteringOffset;
        transform.position = ApplyRubberBorders(transform.position);
    }

    private void HandleBuildingTap(Vector2 screenPosition)
    {
        Ray ray = cam.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            clickBuildings building = hit.transform.GetComponent<clickBuildings>();
            if (building != null)
            {
                building.OnClick();
            }
        }
    }

    // Остальные методы без изменений

    private Vector3 ApplyRubberBorders(Vector3 targetPos)
    {
        Vector3 overflow = CalculateOverflow(targetPos);
        if (overflow.magnitude <= 0.01f)
        {
            return targetPos;
        }

        if (checkEdge(targetPos))
        {
            float slowdown = CalculateSlowdown(overflow);
            return Vector3.Lerp(transform.position, targetPos, slowdown);
        }
        else
        {
            Vector3 clampedPos = GetClampedPosition(targetPos);
            return Vector3.Lerp(transform.position, clampedPos, edgeStiffness);
        }
    }

    private Vector3 CalculateOverflow(Vector3 position)
    {
        float minX = 0 - lowBorderMargin;
        float maxX = map.mapSize - highBorderMargin;
        float minZ = 0 - lowBorderMargin;
        float maxZ = map.mapSize - highBorderMargin;
        Vector3 overflow = Vector3.zero;
        if (position.x < minX) overflow.x = position.x - minX;
        else if (position.x > maxX) overflow.x = position.x - maxX;
        if (position.z < minZ) overflow.z = position.z - minZ;
        else if (position.z > maxZ) overflow.z = position.z - maxZ;
        return overflow;
    }

    private float CalculateSlowdown(Vector3 overflow)
    {
        float normalizedOverflow = Mathf.Clamp01(
            Mathf.Max(Mathf.Abs(overflow.x), Mathf.Abs(overflow.z)) / slowdownStartDistance
        );
        return Mathf.Lerp(1f, maxSlowdownFactor, normalizedOverflow);
    }

    private Vector3 GetClampedPosition(Vector3 position)
    {
        float minX = 0 - lowBorderMargin;
        float maxX = map.mapSize - highBorderMargin;
        float minZ = 0 - lowBorderMargin;
        float maxZ = map.mapSize - highBorderMargin;
        Vector3 clamped = position;
        if (position.x < minX) clamped.x = minX - (minX - position.x) * 0.5f;
        else if (position.x > maxX) clamped.x = maxX + (position.x - maxX) * 0.5f;
        if (position.z < minZ) clamped.z = minZ - (minZ - position.z) * 0.5f;
        else if (position.z > maxZ) clamped.z = maxZ + (position.z - maxZ) * 0.5f;
        return clamped;
    }

    private void ReturnToBounds()
    {
        Vector3 targetPos = new Vector3(
            Mathf.Clamp(transform.position.x, 0 - lowBorderMargin, map.mapSize - highBorderMargin),
            transform.position.y,
            Mathf.Clamp(transform.position.z, 0 - lowBorderMargin, map.mapSize - highBorderMargin)
        );
        float currentSpeed = checkEdge(transform.position) ? returnSpeedInside : returnSpeedOutside;
        transform.position = Vector3.Lerp(transform.position, targetPos, currentSpeed * Time.deltaTime);
    }

    public void panToPosition(Vector3 targetLocation)
    {
        Vector3 direction = (targetLocation - transform.position).normalized;
        transform.position += direction * panningSpeed * Time.deltaTime;
        if (Vector3.Distance(transform.position, targetLocation) < 0.1f)
        {
            pan = false;
        }
    }

    private bool checkBorders(Vector3 t)
    {
        return t.x >= 0 - lowBorderMargin &&
               t.z >= 0 - lowBorderMargin &&
               t.x <= map.mapSize - highBorderMargin &&
               t.z <= map.mapSize - highBorderMargin;
    }

    private bool checkEdge(Vector3 t)
    {
        return t.x >= 0 - lowBorderMargin - slowdownStartDistance &&
               t.z >= 0 - lowBorderMargin - slowdownStartDistance &&
               t.x <= map.mapSize - highBorderMargin + slowdownStartDistance &&
               t.z <= map.mapSize - highBorderMargin + slowdownStartDistance;
    }

    private Vector3 GetWorldPos(float y) => GetWorldPos(y, Input.mousePosition);
    private Vector3 GetWorldPos(float y, Vector2 screenPosition)
    {
        Ray mousePos = cam.ScreenPointToRay(screenPosition);
        Plane ground = new Plane(Vector3.down, new Vector3(0, y, 0));
        float distance;
        ground.Raycast(mousePos, out distance);
        return mousePos.GetPoint(distance);
    }
}