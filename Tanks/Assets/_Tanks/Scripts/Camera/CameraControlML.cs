using UnityEngine;

namespace Tanks.Complete
{
    public class CameraControlML : MonoBehaviour
    {
        public float m_DampTime = 0.2f;
        public float m_ScreenEdgeBuffer = 4f;
        public float m_MinSize = 6.5f;
        public Transform[] m_Targets;

        private Camera[] m_Cameras;
        private float m_ZoomSpeed;
        private Vector3 m_MoveVelocity;
        private Vector3 m_DesiredPosition;

        private Vector3 m_AimToRig;

        private void Awake()
        {
            m_Cameras = GetComponentsInChildren<Camera>();
            
            if (m_Cameras.Length > 0)
            {
                Plane p = new Plane(Vector3.up, transform.position);
                Ray r = new Ray(m_Cameras[0].transform.position, m_Cameras[0].transform.forward);
                p.Raycast(r, out float d);

                var aimTarget = r.GetPoint(d);
                m_AimToRig = transform.position - aimTarget;
            }
        }

        private void FixedUpdate()
        {
            Move();
            Zoom();
        }

        private void Move()
        {
            FindAveragePosition();
            transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition + m_AimToRig, ref m_MoveVelocity, m_DampTime);
        }

        private void FindAveragePosition()
        {
            Vector3 averagePos = new Vector3();
            int numTargets = 0;

            for (int i = 0; i < m_Targets.Length; i++)
            {
                if (!m_Targets[i].gameObject.activeSelf)
                    continue;

                averagePos += m_Targets[i].position;
                numTargets++;
            }

            if (numTargets > 0)
                averagePos /= numTargets;

            averagePos.y = transform.position.y;
            
            m_DesiredPosition = averagePos;
        }

        private void Zoom()
        {
            float requiredSize = FindRequiredSize();
            
            foreach (Camera cam in m_Cameras)
            {
                if (cam != null && cam.orthographic)
                {
                    cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
                }
            }
        }

        private float FindRequiredSize()
        {
            if (m_Cameras.Length == 0 || m_Cameras[0] == null)
                return m_MinSize;

            Camera primaryCamera = m_Cameras[0];
            Vector3 desiredLocalPos = primaryCamera.transform.InverseTransformPoint(m_DesiredPosition);

            float size = 0f;

            for (int i = 0; i < m_Targets.Length; i++)
            {
                if (!m_Targets[i].gameObject.activeSelf)
                    continue;

                Vector3 targetLocalPos = primaryCamera.transform.InverseTransformPoint(m_Targets[i].position);
                Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));
                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / primaryCamera.aspect);
            }

            size += m_ScreenEdgeBuffer;
            size = Mathf.Max(size, m_MinSize);

            return size;
        }

        public void SetStartPositionAndSize()
        {
            FindAveragePosition();
            transform.position = m_DesiredPosition;
            
            float requiredSize = FindRequiredSize();
            
            foreach (Camera cam in m_Cameras)
            {
                if (cam != null && cam.orthographic)
                {
                    cam.orthographicSize = requiredSize;
                }
            }
        }
    }
}
