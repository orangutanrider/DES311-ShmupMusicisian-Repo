using UnityEngine;
using AK.Wwise;

public class EnemyBulletBasicNote : EnemyBulletMusicalNote, IEnemyBulletActivatable
{
    // xPosition - Pitch
    // Angle - PWM
    // Speed - Transpose

    [Header("Required References")]
    [SerializeField] EnemyBulletBasicNoteParams noteParams;
    public Rigidbody2D rb2D;

    public override IEnemyBulletActivatable GetActivationInterface() { return this; }

    private void Update()
    {
        RTPC volume = noteParams.volumeRTPC;
        volume.SetValue(gameObject, Mathf.Lerp(0, 100, BulletEnvelope.envelopeObj.Current01Value));
    }

    void IEnemyBulletActivatable.Activate()
    {
        // Get Params
        RTPC pitch = noteParams.pitchRTPC;
        RTPC pwm = noteParams.pwmRTPC;
        RTPC transpose = noteParams.transposeRTPC;
        RTPC volume = noteParams.volumeRTPC;

        // for the vector2 range variables on the noteparams object x is min y is max
        // formula is: 01Range = (value - min) / (max - min)
        // the value is the variable being put into it (i.e. currentRotation, currentSpeed, currentXPosition, ect.)
        float xPositionRange01 = (transform.position.x - noteParams.pitchXPositionRange.x) / (noteParams.pitchXPositionRange.y - noteParams.pitchXPositionRange.x);
        float angleRange01 = (bulletRoot.transform.rotation.eulerAngles.z - noteParams.pwmAngleRange.x) / (noteParams.pwmAngleRange.y - noteParams.pwmAngleRange.x);
        float speedRange01 = (rb2D.velocity.magnitude - noteParams.transposeSpeedRange.x) / (noteParams.transposeSpeedRange.y - noteParams.transposeSpeedRange.x);

        // Set Values 
        pitch.SetValue(gameObject, Mathf.Lerp(0, 100, xPositionRange01));
        pwm.SetValue(gameObject, Mathf.Lerp(0, 100, angleRange01));
        transpose.SetValue(gameObject, Mathf.Lerp(0, 100, speedRange01));

        // Begin Envelope Volume Lerp
        BulletEnvelope.envelopeObj.TriggerEnvelopeCoroutineLerp(this);
        volume.SetValue(gameObject, Mathf.Lerp(0, 100, BulletEnvelope.envelopeObj.Current01Value));
    }
}
