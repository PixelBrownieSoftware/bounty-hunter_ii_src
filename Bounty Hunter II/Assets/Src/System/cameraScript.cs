using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cameraScript : MonoBehaviour {

    public GameObject player;
    public float movtime;
    public float speed;

    private float camerashakeDel;
    private float cameraoffset_X;
    private float cameraoffset_Y;

    public static Vector2 camerastaticshake;
    

    public bool constant_shake;
    public static cameraScript camOptions;

    public Vector3 destination;
    public enum cameraStates { player, cutscene };
    public cameraStates camState;

    public Vector3 minCameraBoundaries;
    public Vector3 maxCameraBoundaries;
    Image fad;
    bool isFaded = true;
    Queue<Color> fade_queues = new Queue<Color>();

    public void FadeAdd(Color colour)
    {
        fade_queues.Enqueue(colour);
    }

    public bool GetFaded()
    {
        return isFaded;
    }

    private void Awake()
    {
        if (GameObject.Find("Fade"))
            fad = GameObject.Find("Fade").GetComponent<Image>();
    }

    void Start() {
        speed = 1f;
        movtime = 0f;

        if (camOptions == null)
        {
            camOptions = this.gameObject.GetComponent<cameraScript>();
        }
    }

    public void GetCameraBounds(Vector3 min, Vector3 max, Vector3 PlayerPos)
    {
        Vector3 pos = PlayerPos;
        minCameraBoundaries = min;
        maxCameraBoundaries = max;
        FixCamera(pos);
    }
    /*
    public void updateOrt()
    {
        lastSize = Screen.height;

        // first find the reference orthoSize
        float refOrthoSize = (referenceOrthographicSize / referencePixelsPerUnit) * 0.5f;

        // then find the current orthoSize
        var overRide = FindOverride(lastSize);
        float ppu = overRide != null ? overRide.referencePixelsPerUnit : referencePixelsPerUnit;
        float orthoSize = (lastSize / ppu) * 0.5f;

        // the multiplier is to make sure the orthoSize is as close to the reference as possible
        float multiplier = Mathf.Max(1, Mathf.Round(orthoSize / refOrthoSize));

        // then we rescale the orthoSize by the multipler
        orthoSize /= multiplier;

        // set it
        this.GetComponent<Camera>().orthographicSize = orthoSize;
    }
    */
    void FixedUpdate() {
        /* from: https://answers.unity.com/questions/1138789/change-window-size-without-changing-game-resolutio.html */

        if (gui_options.orthgraSize != 0)
        {
            //Camera.main.transform.GetChild(0).GetComponent<Camera>().orthographicSize = gui_options.orthgraSize * gui_options.proport;
            //Camera.main.transform.GetChild(0).GetComponent<Camera>().orthographicSize = Mathf.Min(gui_options.curscreenreslutn.x, gui_options.curscreenreslutn.y)/2;
            //Camera.main.transform.GetChild(0).GetChild(0).transform.localScale = new Vector2(gui_options.curscreenreslutn.x, gui_options.curscreenreslutn.y);
        }
        
        switch (camState) {

            case cameraStates.player:

                //player = GameObject.FindGameObjectWithTag("Player");
                break;

        }

        if (Input.GetKeyDown(KeyCode.M)) {
            if (SoundManager.SFX.music.volume > 0) {
                SoundManager.SFX.music.volume = 0;
            } else {
                SoundManager.SFX.music.volume = 1;
            }
        }
        if (fade_queues.Count > 0)
        {
            if (isFaded)
                StartCoroutine(Fade(fade_queues.Peek()));
        }

        if (Screen.fullScreen)
        {
            if (Input.GetKeyDown(KeyCode.F4))
            {
                Screen.fullScreen = false;
            }
        }
        else {

            if (Input.GetKeyDown(KeyCode.F4))
            {
                Screen.fullScreen = true;
            }
        }

        if (movtime < speed) {

            movtime = +Time.deltaTime;
            float s = movtime / speed;
            transform.position = new Vector3(transform.position.x, transform.position.y, -4);

            if (player != null)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(Mathf.Clamp(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(minCameraBoundaries.x), Mathf.RoundToInt(maxCameraBoundaries.x))
                    , Mathf.Clamp(Mathf.RoundToInt(player.transform.position.y), Mathf.RoundToInt(minCameraBoundaries.y), Mathf.RoundToInt(maxCameraBoundaries.y)), -23f), s * 6.7f);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, destination, s * 6.7f);
            }
        }
    }


    IEnumerator Fade(Color colour)
    {
        isFaded = false;
        Color former = fad.color;
        //print(colour);
        float time = 0;
        while (fad.color != colour)
        {
            //print(time);
            time += 0.1f;
            fad.color = Color.Lerp(former, colour, time);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        isFaded = true;
        fade_queues.Dequeue();
    }

    void FixCamera(Vector3 pos)
    {
        if (pos.x <= minCameraBoundaries.x)
            pos = new Vector3(minCameraBoundaries.x, transform.position.y);
        if (pos.x >= maxCameraBoundaries.x)
            pos = new Vector3(maxCameraBoundaries.x, transform.position.y);
        if(pos.y <= minCameraBoundaries.y)
            pos = new Vector3(transform.position.y, minCameraBoundaries.y);
        if(pos.y >= maxCameraBoundaries.y)
            pos = new Vector3(transform.position.y, maxCameraBoundaries.y);

        transform.position = pos;
    }

    void Update() {


        if (constant_shake)
        {
            cameraShake(camerastaticshake.x, camerastaticshake.y, 0.2f);
        }

        if (camerashakeDel > 0)
        {

            float cam_off_x = Random.Range(-cameraoffset_X, cameraoffset_X);
            float cam_off_y = Random.Range(-cameraoffset_Y, cameraoffset_Y);

            transform.position = new Vector3(transform.position.x + cam_off_x, transform.position.y + cam_off_y, -23);

            camerashakeDel = camerashakeDel - Time.deltaTime;

        }
    } 

    public void cameraShake(float x, float y, float delay)
    {
        cameraoffset_X = x;
        cameraoffset_Y = y;

        camerashakeDel = delay;
    }
}
