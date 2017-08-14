using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterHero : MonoBehaviour {
    [HideInInspector]
    public bool IsMain = false;
    private bool m_Moving = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /*if (IsMain)
        {
            Vector3 dst = transform.position;
            float offset = Time.deltaTime;
            bool move = false;
            if (Input.GetKey(KeyCode.W)) { dst.z += offset; move = true; }
            if (Input.GetKey(KeyCode.S)) { dst.z -= offset; move = true; }
            if (Input.GetKey(KeyCode.A)) { dst.x -= offset; move = true; }
            if (Input.GetKey(KeyCode.D)) { dst.x += offset; move = true; }
            if (move)
            {
                if (!m_Moving) StartMove();
                transform.position = dst;

                Vector3 src = Camera.main.transform.position;
                dst.y = 3;
                dst.z -= 3;
                Camera.main.transform.position = dst;
            }
            else
            {
                if (m_Moving) StopMove();
            }
        }*/
    }

    public void Init()
    {
    }

    public void StartMove()
    {
        m_Moving = true;
        Animator anim = GetComponent<Animator>();
        anim.Play("Run");
    }

    public void StopMove()
    {
        m_Moving = false;
        Animator anim = GetComponent<Animator>();
        anim.Play("Idle");
    }

    public void MoveBy(float x, float z)
    {
        Vector3 pos = transform.position;
        pos.x -= x * 0.05f;
        pos.z -= z * 0.05f;
        transform.position = pos;
        FaceTo(x, z);
    }

    public void FaceTo(float x, float z)
    {
        float angle = Vector2.SignedAngle(Vector2.down, new Vector2(x, z));
        transform.rotation = Quaternion.AngleAxis(angle, Vector2.down);
    }
}
