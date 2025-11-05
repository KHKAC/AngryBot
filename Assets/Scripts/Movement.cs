using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
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

    // 이동 속도
    [SerializeField] float moveSpeed = 10.0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        camera = Camera.main;

        plane = new Plane(transform.up, transform.position);
    }

    void Update()
    {
        Move();
        Turn();
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
}
