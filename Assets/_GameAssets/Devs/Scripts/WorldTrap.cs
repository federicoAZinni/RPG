using UnityEngine;

namespace RPG
{
    public class WorldTrap : MonoBehaviour
    {
        enum TrapType { Stationary, OpenClose, TrackMovement  }

        enum DamageType { ByCollision, ByTime }

        [SerializeField] bool enabledAtStart = true;
        [SerializeField] TrapType trapType;
        [SerializeField] float damageToDeal, timeToDealDamage;
        [SerializeField] DamageType damageType;
        [SerializeField] Bounds damageBounds;
        [SerializeField] LayerMask damageMask;
        [SerializeField] float timeToOpen, timeToClose, openSpeed, closeSpeed;
        [SerializeField] Transform openedPosition, closedPosistion;
        [SerializeField] float trackMoveSpeed;

        public bool TrapEnabled { get; private set; }

        bool isTrapOpened;
        int currentTrackIndex, trackMovementDir;
        float damageTimer, openCloseTimer;
        Vector3 openedPoint, closedPoint;
        Vector3[] trackPoints;
        Collider[] possibleTargets;

        void Awake()
        {
            possibleTargets = new Collider[20];

            switch (trapType)
            {
                case TrapType.OpenClose:
                    openedPoint = openedPosition.position;
                    closedPoint = closedPosistion.position;

                    Destroy(openedPosition.gameObject);
                    Destroy(closedPosistion.gameObject);
                    break;

                case TrapType.TrackMovement:
                    Transform[] trackTransforms = transform.GetComponentsInChildren<Transform>();
                    int size = trackTransforms.Length;

                    trackPoints = new Vector3[size];
                    for (int i = 0; i < size; i++)
                    {
                        trackPoints[i] = trackTransforms[i].position;
                        Destroy(trackTransforms[i].gameObject);
                    }
                    break;
            }
        }

        void Start()
        {
            TrapEnabled = enabledAtStart;
            damageTimer = Time.time + timeToDealDamage;
        }

        void Update()
        {
            if (!TrapEnabled) return;

            switch (trapType)
            {
                case TrapType.OpenClose: OpenCloseBehaviour(); break;
                case TrapType.TrackMovement: TrackMovementBehaviour(); break;
            }

            switch (damageType)
            {
                case DamageType.ByCollision: HanldeDamageByCollision(); break;
                case DamageType.ByTime: HandleDamageByTime(); break;
            }
        }

        void OpenCloseBehaviour()
        {
            if (isTrapOpened)
            {
                if (openCloseTimer > Time.time) return;
                
                if (Vector3.Distance(transform.position, closedPoint) < Vector3.kEpsilon)
                    openCloseTimer = Time.time + timeToOpen;

                transform.position = Vector3.MoveTowards(transform.position, closedPoint, closeSpeed);
                return;
            }

            if (openCloseTimer > Time.time) return;

            if (Vector3.Distance(transform.position, closedPoint) < Vector3.kEpsilon)
                openCloseTimer = Time.time + timeToClose;

            transform.position = Vector3.MoveTowards(transform.position, openedPoint, openSpeed);
        }

        void TrackMovementBehaviour()
        {
            if (currentTrackIndex == trackPoints.Length) trackMovementDir = -1;
            else if (currentTrackIndex == 0) trackMovementDir = 1;

            if (Vector3.Distance(transform.position, trackPoints[currentTrackIndex]) < Vector3.kEpsilon)
                currentTrackIndex = Mathf.Clamp(currentTrackIndex + trackMovementDir, 0, trackPoints.Length);

            transform.position = Vector3.MoveTowards(transform.position, trackPoints[currentTrackIndex], trackMoveSpeed * Time.deltaTime);
        }

        void HanldeDamageByCollision()
        {

        }

        void HandleDamageByTime()
        {
            if (damageTimer > Time.time) return;

            int quantity = Physics.OverlapBoxNonAlloc(transform.position + damageBounds.center, damageBounds.extents, possibleTargets, transform.rotation, damageMask);
            for (int i = 0; i < quantity; i++)
            {
                IDamageable target = possibleTargets[i].GetComponent<IDamageable>();
                if (target == null || target.HP <= 0) return;
                target.Damage(damageToDeal);
            }

            damageTimer = Time.time + timeToDealDamage;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(damageBounds.center, damageBounds.size);
        }
    }
}
