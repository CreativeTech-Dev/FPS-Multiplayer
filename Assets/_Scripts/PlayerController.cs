using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] GameObject cameraHolder;

    [SerializeField] float mouseSens, sprintSpeed, walkSpeed, jumpForce, smoothTime;
    bool grounded;
    float verticalLookRotation;
    Vector3 smoothMoveVel;
    Vector3 moveAmount;

    [SerializeField] LayerMask groundlayer;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.1f;
    Rigidbody rb;
    Vector3 movedir;

    PhotonView pv;

    [SerializeField] Item[] items;
    int itemIndex;
    int prevItemIndex = -1;

    PlayerManager playerManager;
    float maxHP = 100f;
    float currentHP;
    [SerializeField] float levelMinYValue = -35f;
    [SerializeField] Image hpImageBar;
    [SerializeField] GameObject ui;

    [SerializeField] Image crosshairImg;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();

        playerManager = PhotonView.Find((int)pv.InstantiationData[0]).GetComponent<PlayerManager>();
        currentHP = maxHP;
    }

    private void Start() {
        if (!pv.IsMine) {
            Destroy(ui);
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
        }
        else {
            EquipItem(0);
            GetCrosshair();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!pv.IsMine) {
            return;
        }
        LookControlls();
        Move();
        Jump();

        for (int i = 0; i < items.Length; i++) {
            if (Input.GetKeyDown((i+1).ToString())) {
                EquipItem(i);
                break;
            }
        }
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f) {
            EquipItem((itemIndex + 1) % items.Length);
        }
        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f) {
            if (itemIndex == 0) {
                itemIndex = items.Length;
            }
            EquipItem(itemIndex - 1);
        }
        if (Input.GetMouseButtonDown(0)) {
            items[itemIndex].Use();
        }
    }

    private void FixedUpdate() {
        if (pv.IsMine) {
            grounded = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, groundlayer).Length > 0;
            //rb.velocity = moveAmount;
            rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);

            if (transform.position.y < levelMinYValue) {
                Die();
            }
        }
    }
    void Jump() {
        
        if (Input.GetButtonDown("Jump") && grounded) {
            rb.AddForce(transform.up * jumpForce);
        }
    }
    void Move() {
        movedir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        moveAmount = Vector3.SmoothDamp(moveAmount, movedir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVel, smoothTime);
    }
    void LookControlls() {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSens);
        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSens;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -45f, 80f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    void EquipItem(int _index) {
        if (prevItemIndex == _index) {
            return;
        }

        itemIndex = _index;
        items[itemIndex].itemGameObject.SetActive(true);

        if (prevItemIndex != -1) {
            items[prevItemIndex].itemGameObject.SetActive(false);
        }
        prevItemIndex = itemIndex;

        if (pv.IsMine) {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        if (changedProps.ContainsKey("itemIndex") && !pv.IsMine && targetPlayer == pv.Owner) {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }

    public void TakeDamage(float damageAmount) {
        pv.RPC(nameof(RPC_TakeDamage), pv.Owner, damageAmount);
    }


    [PunRPC]
    void RPC_TakeDamage(float damageAmount, PhotonMessageInfo info) {
        currentHP -= damageAmount;
        hpImageBar.fillAmount = currentHP/maxHP;

        if (currentHP <= 0) {
            Die();
            PlayerManager.Find(info.Sender).GetKill();
        }
    }

    void GetCrosshair() {
        if (PlayerPrefs.HasKey("crosshairImages")) {
            crosshairImg.sprite = CrosshairManager.Instance.crosshairImages[PlayerPrefs.GetInt("crosshairImages")];
        }
        if (PlayerPrefs.HasKey("crosshairColor")) {
            crosshairImg.color = CrosshairManager.Instance.crosshairColors[PlayerPrefs.GetInt("crosshairColor")];
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Die() {
        playerManager.Die();
    }
}
