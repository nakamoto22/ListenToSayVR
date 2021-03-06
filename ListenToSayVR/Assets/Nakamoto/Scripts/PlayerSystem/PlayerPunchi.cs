﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Player/Hand/PlayerPunchi")]
public class PlayerPunchi : MonoBehaviour
{
    //---------------------------------------------------------------------------------------------
    //  定数
    //---------------------------------------------------------------------------------------------

    //Contorollerの移動速度の基準値
    const float HAND_SPEED_NORM = 2.0f;
    //パンチ時の拳の角度の基準値
    const float PUNCHI_DIR_NORM = 20.0f;

    //---------------------------------------------------------------------------------------------
    //  Private
    //---------------------------------------------------------------------------------------------

    //Transform
    Transform transformCache;
    //Audio
    new AudioSource audio;
    //Contolloer
    [SerializeField]
    OVRInput.Controller handState;
    //前方方向
    [SerializeField]
    Transform dirPoint;
    //EffectのPrefab
    [SerializeField]
    HandEffect handEfePre;
    //効果音 0.素振り 3.空気砲　2.殴打
    [SerializeField, Space]
    AudioClip[] handSound = new AudioClip[3];

    //殴られた敵のRigidbody
    Rigidbody HitObjRig;

    //力のVector
    Vector3 handVelocity = Vector3.zero;
    //力の大きさ
    float handPower = 0;
    float handPower_before;

    //---------------------------------------------------------------------------------------------
    //  enum (Sound)
    //---------------------------------------------------------------------------------------------

    enum SoundState
    {
        Swing = 0,
        Shot,
        Punch,
    }

    //---------------------------------------------------------------------------------------------
    //  Start
    //---------------------------------------------------------------------------------------------
    void Start ()
    {
        //Transformを取得
        transformCache = transform;
        //AudioSourceを取得
        audio = transformCache.GetComponent<AudioSource>();
	}
    //---------------------------------------------------------------------------------------------
    //  Update
    //---------------------------------------------------------------------------------------------
    void Update()
    {
        HandMoveVelocity();
    }
    //---------------------------------------------------------------------------------------------
    //  HandMove
    //---------------------------------------------------------------------------------------------
    void HandMoveVelocity()
    {
        //値の取得
        handVelocity = OVRInput.GetLocalControllerVelocity(handState);
        handPower_before = handPower;
        handPower = handVelocity.magnitude;

        //一定速度以下
        if (!IsPubchEffectCrete()) return;

        //手を生成
        //HandEffect handEfe = Instantiate(handEfePre, transformCache.position, transformCache.rotation);
        //handEfe.SetPower(handVelocity * 100);
    }
    bool IsPubchEffectCrete()
    {
        if (handPower_before < HAND_SPEED_NORM) return false;
        if (handPower > HAND_SPEED_NORM) return false;
        if (!IsPunchiDirCheck())
        {
            //音再生
            HandMoveSound(SoundState.Swing);
            return false;
        }
        //音再生
        HandMoveSound(SoundState.Shot);
        return true;
    }
    //パンチの角度が基準値以下ならtrue
    bool IsPunchiDirCheck()
    {
        //Vector
        Vector3 handToPoint = dirPoint.position - transformCache.position;
        //角度
        float angle = Vector3.Angle(handToPoint, handVelocity);
        //返り値
        if (angle > PUNCHI_DIR_NORM) return false;
        return true;
    }
    //---------------------------------------------------------------------------------------------
    //  当たり判定
    //---------------------------------------------------------------------------------------------
    void OnTriggerEnter(Collider collider)
    {
        if(collider.transform.tag == "Enemy")
        {
            HitHand(collider);
        }
    }
    void HitHand(Collider coll)
    {
        //一定速度以下
        if (handPower < HAND_SPEED_NORM) return;

        //殴った
        HitObjRig = coll.gameObject.GetComponent<Rigidbody>();
        Enemy_Master enemy = coll.GetComponent<Enemy_Master>();
        enemy.HitPunch(handVelocity);

        //音再生
        HandMoveSound(SoundState.Punch);
    }

    //---------------------------------------------------------------------------------------------
    //  PunchiSound
    //---------------------------------------------------------------------------------------------
    void HandMoveSound(SoundState state)
    {
        //再生中ならreturn
        if (state == SoundState.Swing && audio.isPlaying) return;
        //AudioClipをセット
        audio.clip = handSound[(int)state];
        //音再生
        audio.Play();
    }
}
