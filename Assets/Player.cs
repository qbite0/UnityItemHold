using UnityEngine;

//EasyCode Example))
public class Player : MonoBehaviour
{
    [Header("Camera")]
    public float Sensitivity; //Mouse Sensivity
    public Vector2 Rotation; //Rotation Vector

    [Header("Movement")]
    public float Speed = 5f; //Movement Speed

    [Header("Other")]
    public GameObject HoldingItem; //Current Holding item gameobject
    public float HoldingDistance; //Distance from kotoroy i'm hold item
    public Transform Holder; //Item hold position

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //Locing and hide cursor
    }

    private void Update()
    {
        Move();
        PlayerCamera();
        HoldItems();
    }

    void Move()
    {
        //I don't make gravity because this shit (Controller.isGrounded) very lags
        Vector3 Movement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")); //Move vector
        transform.rotation = Quaternion.Euler(0, Rotation.x, 0); //Rotate Player with camera
        transform.Translate(Movement * Time.deltaTime * Speed); //Move Player
    }

    void PlayerCamera()
    {
        Rotation += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * Sensitivity; //Rotation vector * Sensivity
        Rotation.y = Mathf.Clamp(Rotation.y, -90f, 90f); //Clamping Rotation by Y
        Camera.main.transform.rotation = Quaternion.Euler(-Rotation.y, Rotation.x, 0); //Set Main Camera Rotation
        //U can also use public Camera PlayerCamera or Transform to change rotation value
    }

    void HoldItems()
    {
        //Der Code wird kürzer))
        Ray Ray = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0)); //Fucc camera ray
        //For genius: Ray Ray = Camera.main.ScreenPointToRay(Input.mousePosition)
        Debug.DrawRay(Ray.origin, Ray.direction * HoldingDistance, Color.red); //For RAY debugging

        RaycastHit Hit;

        if (Input.GetKeyDown("f")) { //Press F to hold item
            if (HoldingItem == null) {
                if (Physics.Raycast(Ray, out Hit, HoldingDistance, LayerMask.GetMask("Items"))) { //Hit bool also check items layer
                    HoldingItem = Hit.collider.gameObject; //Set Holding Item
                    HoldingItem.GetComponent<Rigidbody>().useGravity = false; //Disable gravity
                }
            } else {
                Rigidbody rb = HoldingItem.GetComponent<Rigidbody>();
                rb.useGravity = true; //Enable gravity
                rb.AddForce((Holder.position - HoldingItem.transform.position) * 4f, ForceMode.VelocityChange); //Item force
                HoldingItem = null; //Set Holding Item
            }
        }

        if (HoldingItem != null)
        {
            HoldingItem.GetComponent<Rigidbody>().velocity = Vector3.zero; //Reset velocity to fix lags
            HoldingItem.transform.rotation = Holder.transform.rotation; //Set Item rotation
            HoldingItem.transform.position = Vector3.Lerp(HoldingItem.transform.position, Holder.transform.position, Time.deltaTime * 10f); //Smooth item movement
        }
    }
}
