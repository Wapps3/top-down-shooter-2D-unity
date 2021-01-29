using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{
    public Button ReplayButton;
    public Button ExitButton;

    public List<Text> LeaderBoardScore;

    // Start is called before the first frame update
    void Start()
    {
        ReplayButton.onClick.AddListener(Replay);
        ExitButton.onClick.AddListener(Exit);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Replay()
    {

    }

    void Exit()
    {

    }
}
