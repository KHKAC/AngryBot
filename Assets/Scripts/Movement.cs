using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class Movement : MonoBehaviourPunCallbacks, IPunObservable
{
    // 컴포넌트 캐시처리를 위한 변수
    CharacterController controller;
    new Transform transform;
    Animator animator;
    new Camera camera;

    // 가상의 Plane에 Raycasting하기 위한 변수
    Plane plane;
    Ray ray;
    Vector3 hitPoint;
    PhotonView pv;
    CinemachineVirtualCamera virtualCamera;

    // 이동 속도
    [SerializeField] float moveSpeed = 10.0f;

    Vector3 receivePos;
    Quaternion receiveRot;
    public float damping;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        camera = Camera.main;

        pv = GetComponent<PhotonView>();
        virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();

        if (pv.IsMine)
        {
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;
        }
        
        plane = new Plane(transform.up, transform.position);
    }

    void Update()
    {
        if (pv.IsMine)
        {
            Move();
            Turn();
        }
        else
        {
            // 수신된 좌표로 보간한 이동 처리
            transform.position = Vector3.Lerp(transform.position, receivePos, Time.deltaTime * damping);
            // 수신된 회전값으로 보간한 회전 처리
            transform.rotation = Quaternion.Slerp(transform.rotation, receiveRot, Time.deltaTime * damping);
        }
    }

    float h => Input.GetAxis("Horizontal");
    float v => Input.GetAxis("Vertical");

    // 이동 처리하는 함수
    void Move()
    {
        Vector3 cameraForward = camera.transform.forward;
        Vector3 cameraRight = camera.transform.right;
        cameraForward.y = 0.0f;
        cameraRight.y = 0.0f;

        // 이동할 방향 벡터 계산
        Vector3 moveDir = (cameraForward * v) + (cameraRight * h);
        moveDir.Set(moveDir.x, 0.0f, moveDir.z);

        // 주인공 캐릭터 이동 처리
        controller.SimpleMove(moveDir * moveSpeed);

        // 주인공 캐릭터의 애니메이션 처리
        float forward = Vector3.Dot(moveDir, transform.forward);
        float strafe = Vector3.Dot(moveDir, transform.right);

        animator.SetFloat("Forward", forward);
        animator.SetFloat("Strafe", strafe);
    }

    // 회전 처리하는 함수
    void Turn()
    {
        // 마우스의 2차원 좌표값을 3차원 광선(레이)을 생성
        ray = camera.ScreenPointToRay(Input.mousePosition);

        float enter = 0.0f;

        // 가상의 바닥에 ray를 발사해 충돌한 지점의 거리를 enter 변수로 변환
        plane.Raycast(ray, out enter);
        //가상의 바닥에 레이가 충돌한 좌표값 추출
        hitPoint = ray.GetPoint(enter);

        // 회전해야 할 방향의 벡터를 계산
        Vector3 lookDir = hitPoint - transform.position;
        lookDir.y = 0;
        // 주인공 캐릭터의 회전값 지정
        transform.localRotation = Quaternion.LookRotation(lookDir);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 자신이 로컬 캐릭터인 경우 자신의 데이터를 다른 네트워크에 송신
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            receivePos = (Vector3)stream.ReceiveNext();
            receiveRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
