using UnityEngine;
using System.Collections;

public class PlayerAnimator : MonoBehaviour {

    private Animator playerAnim;

    #region Animator_HashTag
    private static int doIdleHash = Animator.StringToHash("doIdle");
    private static int isAttackingHash = Animator.StringToHash("isAttacking");
    private static int idleRandomHash = Animator.StringToHash("idleRandom");
    private static int movingHash = Animator.StringToHash("isMoving");
    private static int directionHash = Animator.StringToHash("direction");
    private static int pitchHash = Animator.StringToHash("pitch");
    private float idleChangeProbability = 0.2f;
    private int idleCountAnim = 4;
    #endregion

    // Use this for initialization
    void Start() {
        playerAnim = transform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
    }

    public void Attack() {
        this.playerAnim.SetTrigger("isAttackingTrigger");
    }

    public void SetMoving(bool isMoving)
    {
        this.playerAnim.SetBool(movingHash, isMoving);
    }

    public void SetDirection(float newDirection) {
        this.playerAnim.SetFloat(directionHash, newDirection);
    }

    public float GetDirection() {
        return this.playerAnim.GetFloat(directionHash);
    }

    public void SetPitch(float newPitch) {
        this.playerAnim.SetFloat(pitchHash, newPitch);
    }

    public float GetPitch() {
        return this.playerAnim.GetFloat(pitchHash);
    }

    #region EventFuntions
    public void DoIdle() //Called in idle animation
    {
        float doNewIdleProb = Random.Range(0f, 1f);
        if (doNewIdleProb < idleChangeProbability) {
            float randomTime = Random.Range(0f, 2f);
            Invoke("DoIdleInTime", randomTime);
        }
    }

    private void DoIdleInTime() {
        int newIdleIndex = Random.Range(1, idleCountAnim);
        playerAnim.SetBool(doIdleHash, true);
        playerAnim.SetInteger(idleRandomHash, newIdleIndex);
    }

    //sharkAnimStateInfo

    public void AttackEvent() //Called in attack animation
    {
        //biteDetector.SetActive(true);
        //biteDetector.GetComponent<BiteDectector>().Baiting = true;
        //meatDetector.SetActive(true);

        Invoke("DisableAttack", 0.1f);
    }

    public void AttackSharkWhale() {
        //meatDetector.SetActive(true);
    }

    public void AttackSqueeze() //Called in attack shake loop animation
    {
        //biteDetector.SetActive(true);
        //biteDetector.GetComponent<BiteDectector>().BaitSqueezing = true;

        Invoke("DisableAttack", 0.2f);
    }

    public void DisableAttack() //Called by Invoke method. Also called in the attack whale shark (at the end)
    {
        //biteDetector.SetActive(false);
        //biteDetector.GetComponent<BiteDectector>().Baiting = false;
        //biteDetector.GetComponent<BiteDectector>().BaitSqueezing = false;
        //meatDetector.SetActive(false);
    }
    #endregion
}
