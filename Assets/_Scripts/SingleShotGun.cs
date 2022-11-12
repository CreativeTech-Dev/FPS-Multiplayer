using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGun : Gun
{
    [SerializeField] Camera cam;
    PhotonView pv;

    public override void Use() {
        Shoot();
    }
    private void Awake() {
        pv = GetComponent<PhotonView>();
    }

    private void Start() {
        if (cam == null) {
            cam = transform.parent.parent.GetComponentInChildren<Camera>();
        }
    }

    private void Shoot() {

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            pv.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        }
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPos, Vector3 hitNormal) {
        Collider[] colliders = Physics.OverlapSphere(hitPos, 0.3f);
        if (colliders.Length > 0) {
            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPos + hitNormal * 0.001f, Quaternion.LookRotation(-hitNormal, Vector3.up));
            Destroy(bulletImpactObj, 8f);
            bulletImpactObj.transform.SetParent(colliders[0].transform);
        }
    }
}
