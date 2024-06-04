using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    Animator an;
    [SerializeField] Transform CameraTransform;
    
    public float MovementSpeed = 5f;
    float VerticalInput, HorizontalInput;
    float MouseX, MouseY;
    public float Sensitivity = 50f;
    public TMP_Text NameText;
    public PhotonView MyPhotonView;
    public GameObject ScreenSpaceCanvas;
    float Health=100;
    Camera PlayerCamera;
    public Transform Reticle;
    public LayerMask PlayerLayer;
    public UnityEngine.UI.Slider HealthBarSlider;
    public TMP_Text MessageTMP;
    Transform GridTransform;
    
    void Start()
    {
        GridTransform = GameObject.FindGameObjectWithTag("GridLayout").transform;
        PlayerCamera = CameraTransform.GetComponent<Camera>();
        rb = GetComponent<Rigidbody>();
        an = GetComponent<Animator>();
        if (MyPhotonView.isMine)
        {
            NameText.text = PhotonNetwork.player.NickName;
            NameText.gameObject.SetActive(false);
        }
        else
        {
            NameText.text = MyPhotonView.owner.NickName;
            CameraTransform.gameObject.SetActive(false);
            ScreenSpaceCanvas.SetActive(false);
        }
        
    }

    void Update()
    {
        if (MyPhotonView.isMine)
        {
            if (Health > 0)
            {
                MouseX = Input.GetAxis("Mouse X");

                HorizontalInput = Input.GetAxis("Horizontal");
                VerticalInput = Input.GetAxis("Vertical");

                transform.Rotate(0, MouseX * Sensitivity * Time.deltaTime, 0);

                MouseY -= Input.GetAxis("Mouse Y") * Sensitivity * Time.deltaTime;
                MouseY = Mathf.Clamp(MouseY, -15, 22);

                CameraTransform.localRotation = Quaternion.Euler(MouseY, 0, 0);

                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    an.SetTrigger("Fire");
                }


                if (Input.GetKey(KeyCode.LeftShift))
                {
                    HorizontalInput *= 2;
                    VerticalInput *= 2;
                }


                an.SetFloat("SpeedX", HorizontalInput);
                an.SetFloat("SpeedZ", VerticalInput);
            }
            else
            {
                an.SetBool("Death", true);
            }
        }
    }

    private void FixedUpdate()
    {
        if (MyPhotonView.isMine)
        {
            Vector3 movementVector = new Vector3(HorizontalInput * MovementSpeed, rb.velocity.y, VerticalInput * MovementSpeed);
            rb.velocity = transform.TransformDirection(movementVector);//transform.forward*VerticalInput + tranform.right*HorizontalInput;
        }
    }

    void Shoot(float DamageAmount)
    {
        if (MyPhotonView.isMine)
        {
            Ray ReticleRay = PlayerCamera.ScreenPointToRay(Reticle.position);
            if (Physics.Raycast(ReticleRay, out RaycastHit hitObject, 500, PlayerLayer))
            {
                if (hitObject.transform.TryGetComponent(out PhotonView OtherPhotonView))
                    OtherPhotonView.RPC("TakeDamage", PhotonTargets.All, DamageAmount);
            }
        }
    }

    public void SendMessage(TMP_InputField messageIF)
    {
        if (messageIF.text.Length > 0)
        {
            string message = PhotonNetwork.player.NickName + ": " + messageIF.text;
            MyPhotonView.RPC("SendNetworkMessage", PhotonTargets.AllBufferedViaServer, message);
        }
    }

    [PunRPC]
    public void TakeDamage(float Damage)
    {
        if (Health > 0)
        {
            Health -= Damage;
            HealthBarSlider.value = Health;
        }
        
    }

    [PunRPC]
    void SendNetworkMessage(string message)
    {
        TMP_Text messageText=Instantiate(MessageTMP, GridTransform.position, Quaternion.identity);
        messageText.text = message;
        messageText.transform.parent = GridTransform;

        Destroy(messageText.gameObject,3f);

    }
}
