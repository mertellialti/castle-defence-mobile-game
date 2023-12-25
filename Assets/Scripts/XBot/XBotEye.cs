using System;
using Unity.VisualScripting;
using UnityEngine;

public class XBotEye : MonoBehaviour
{
    private Vector3 _rayOrigin;
    private Vector3 _rayDirection;

    private GameObject _obstacle;

    private LayerMask _layerMask;

    private float combatDistance = 2f;
    private float detectDistance = 7;
    private float _counter = 0;
    private float _targetTime = .25f;

    private int _xBotLayer;

    private bool _isBlue;
    private bool _isInCombat;
    public bool enemyInPath;
    public bool enemyInCombatRange;
    
    public XBotLogic enemyLogic;
    public XBotHealth enemyHealth;


    private void Start()
    {
        _layerMask = LayerMask.GetMask("Friendly", "Enemy", "Default");
    }

    private void Update()
    {
        if (enemyLogic)
            if(!enemyLogic.isDead)
                return;

        // Define the ray origin and direction
        _rayOrigin = transform.position;
        _rayDirection = transform.forward;
        // _rayDirection.x = 0;
        // Debug.DrawRay(_rayOrigin, transform.forward * detectDistance, Color.black, 1f);
        // Debug.DrawRay(_rayOrigin, transform.up * detectDistance, Color.black, 1f);

        if (Physics.Raycast(_rayOrigin, _rayDirection, out var hit, detectDistance, _layerMask))
        {
            _obstacle = hit.collider.transform.root.gameObject;

            var obstacleDistance = hit.distance;
            if (obstacleDistance <= combatDistance && IsEnemy(_obstacle.layer))
            {
                if (_obstacle.transform.GetChild(0).TryGetComponent(out XBotLogic xBotLogic))
                    enemyLogic = xBotLogic;
                if (_obstacle.transform.GetChild(0).TryGetComponent(out XBotHealth xBotHealth))
                    enemyHealth = xBotHealth;
                
                Debug.LogWarning("Enemy " + _obstacle.name + "is in combat range.");
                Debug.DrawRay(_rayOrigin, _rayDirection * detectDistance, Color.red, 1.5f);
                enemyInCombatRange = true;
                enemyInPath = true;
            }
            else if (obstacleDistance > combatDistance && IsEnemy(_obstacle.layer))
            {
                Debug.DrawRay(_rayOrigin, _rayDirection * detectDistance, Color.yellow, 1f);
                enemyInPath = true;
                enemyInCombatRange = false;
            }
        }
        else
        {
            Debug.DrawRay(_rayOrigin, _rayDirection * detectDistance, Color.green, 1f);
            enemyInPath = false;
            enemyInCombatRange = false;
        }
    }

    private bool IsEnemy(int layer)
    {
        return layer != gameObject.layer && layer != 0;
    }
}