using UnityEngine;

public class cameraControl : MonoBehaviour
{
    Vector3 touchStart;
    public Camera cam;
    public generateMap map;
    public bool pan = false;
    public float panningSpeed = 1.0f;
    public Vector3 panTo = new Vector3(10, 5, 10);

    public int lowBorderMargin = 3, highBorderMargin = 4;
    public Vector3 mapCenter = new Vector3(12, 0, 12);
    public float pullingForce = 1f;
    public float bufZone = 0.4f;

    public float speed = 0.4f;
    public float MINSCALE = 2F;
    public float MAXSCALE = 5F;
    public float minPinchSpeed = 5.0F;
    public float varianceInDistances = 5.0F;
    private float touchDelta = 0.0F;
    private Vector2 prevDist = new Vector2(0, 0);
    private Vector2 curDist = new Vector2(0, 0);
    private float speedTouch0 = 0.0F;
    private float speedTouch1 = 0.0F;

    private Vector3 dragVelocity;
    private bool wasDragging = false;

    [Header("Rubber Border Settings")]
    public float slowdownStartDistance = 1.5f;
    public float maxSlowdownFactor = 0.3f;
    public float returnSpeedInside = 2.0f;
    public float returnSpeedOutside = 4.0f;
    public float edgeStiffness = 0.8f;

    void Update()
    {
        if (pan)
        {
            panToPosition(panTo);
            return;
        }

        HandleCameraMovement();
    }

    private void HandleCameraMovement()
    {
        // Обработка зума двумя пальцами
        if (Input.touchCount == 2)
        {
            HandlePinchZoom();
            wasDragging = false;
            return;
        }

        // Обработка перетаскивания
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = GetWorldPos(0);
            wasDragging = true;
        }

        if (Input.GetMouseButton(0) && wasDragging)
        {
            Vector3 direction = touchStart - GetWorldPos(0);
            Vector3 desiredPosition = transform.position + direction;

            // Плавное движение с резиновым эффектом
            transform.position = ApplyRubberBorders(desiredPosition);
            touchStart = GetWorldPos(0);
        }

        if (Input.GetMouseButtonUp(0))
        {
            wasDragging = false;
        }

        // Автоматическое возвращение если не тащим и за границами
        if (!wasDragging && !checkBorders(transform.position))
        {
            ReturnToBounds();
        }
    }

    private Vector3 ApplyRubberBorders(Vector3 targetPos)
    {
        // Рассчитываем насколько далеко за границы
        Vector3 overflow = CalculateOverflow(targetPos);

        // Если полностью внутри границ - просто принимаем позицию
        if (overflow.magnitude <= 0.01f)
        {
            return targetPos;
        }

        // Если в буферной зоне - плавное замедление
        if (checkEdge(targetPos))
        {
            float slowdown = CalculateSlowdown(overflow);
            return Vector3.Lerp(transform.position, targetPos, slowdown);
        }
        // Если за буферной зоной - притягиваем к границе
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
            Mathf.Max(Mathf.Abs(overflow.x), Mathf.Abs(overflow.z)) / bufZone
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

        float speed = checkEdge(transform.position) ? returnSpeedInside : returnSpeedOutside;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );
    }

    private void HandlePinchZoom()
    {
        if (Input.GetTouch(1).phase == TouchPhase.Ended)
        {
            touchStart = GetWorldPos(0, 0);
        }
        else if (Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            touchStart = GetWorldPos(0, 1);
        }

        if (Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
        {
            curDist = Input.GetTouch(0).position - Input.GetTouch(1).position;
            prevDist = ((Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition) -
                      (Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition));
            touchDelta = curDist.magnitude - prevDist.magnitude;

            speedTouch0 = Input.GetTouch(0).deltaPosition.magnitude / Input.GetTouch(0).deltaTime;
            speedTouch1 = Input.GetTouch(1).deltaPosition.magnitude / Input.GetTouch(1).deltaTime;

            if ((touchDelta + varianceInDistances <= 1) && (speedTouch0 > minPinchSpeed) && (speedTouch1 > minPinchSpeed))
            {
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + (1 * speed), MINSCALE, MAXSCALE);
            }
            else if ((touchDelta + varianceInDistances > 1) && (speedTouch0 > minPinchSpeed) && (speedTouch1 > minPinchSpeed))
            {
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - (1 * speed), MINSCALE, MAXSCALE);
            }
        }
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
        return t.x >= 0 - lowBorderMargin - bufZone &&
               t.z >= 0 - lowBorderMargin - bufZone &&
               t.x <= map.mapSize - highBorderMargin + bufZone &&
               t.z <= map.mapSize - highBorderMargin + bufZone;
    }

    private Vector3 GetWorldPos(float y)
    {
        Ray mousePos = cam.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.down, new Vector3(0, y, 0));
        float distance;
        ground.Raycast(mousePos, out distance);
        return mousePos.GetPoint(distance);
    }

    private Vector3 GetWorldPos(float y, int touchIndex)
    {
        Ray mousePos = cam.ScreenPointToRay(Input.GetTouch(touchIndex).position);
        Plane ground = new Plane(Vector3.down, new Vector3(0, y, 0));
        float distance;
        ground.Raycast(mousePos, out distance);
        return mousePos.GetPoint(distance);
    }
}