using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class BlockMovement : MonoBehaviour
{
    public float speed = 2f;
    public float exitNudgeDistance = 0.1f; // đẩy nhẹ ra khỏi collider khi vừa teleport

    private Vector2 moveDir = Vector2.right;
    private bool isTeleporting = false;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; //Tránh bỏ sót va chạm
    }

    void FixedUpdate()
    {
        Vector2 targetPos = rb.position + moveDir * speed * Time.fixedDeltaTime;
        rb.MovePosition(targetPos);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Ưu tiên xử lý Goal trước
        if (other.CompareTag("Goal"))
        {
            // Disable block hoàn toàn
            gameObject.SetActive(false);
            return;
        }

        if (isTeleporting) return;

        // Tìm Teleport trên chính collider, cha hoặc con
        Teleport tp = other.GetComponent<Teleport>();
        if (tp == null) tp = other.GetComponentInParent<Teleport>();
        if (tp == null) tp = other.GetComponentInChildren<Teleport>();

        if (tp != null && tp.targetTeleport != null)
        {
            if (tp.targetTeleport.exitPoint == null)
            {
                Debug.LogWarning($"Teleport '{tp.name}' thiếu exitPoint trên '{tp.targetTeleport.name}'. Hãy gán một Transform làm điểm ra.");
                return;
            }
            StartCoroutine(TeleportRoutine(tp));
        }
        else
        {
            Debug.LogWarning(
                $"Không teleport được với collider '{other.name}'. " +
                $"HasTeleport={(tp!=null)} targetTeleport={(tp!=null && tp.targetTeleport!=null)} exitPoint={(tp!=null && tp.targetTeleport!=null && tp.targetTeleport.exitPoint!=null)}");
        }
    }

    IEnumerator TeleportRoutine(Teleport tp)
    {
        isTeleporting = true;

        Vector2 exitPos = tp.targetTeleport.exitPoint.position;

        // Sau khi xuống dưới sẽ đi sang trái
        moveDir *= -1;

        // Đẩy nhẹ ra khỏi trigger theo hướng mới
        Vector2 nudge = moveDir.normalized * exitNudgeDistance;
        rb.position = exitPos + nudge;
        transform.position = rb.position;

        //Đợi 1 frame vật lý
        yield return new WaitForFixedUpdate();

        float t = 0f, timeout = 0.2f;
        while (t < timeout)
        {
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isTeleporting = false;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Teleport>() != null || other.GetComponentInParent<Teleport>() != null)
        {
            isTeleporting = false;
        }
    }
}
