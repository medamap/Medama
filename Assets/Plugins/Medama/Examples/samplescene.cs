using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Medama.EUGML;

public class samplescene : MonoBehaviour {

	void Start () {

        var dc = EUGML.MedamaUIParseXml(
@"<?xml version='1.0'?>
<uGUI xmlins='http://megamin.jp/ns/unity3d/ugui/eugml'>

<!-- Scenes -->
  <AddNode name='Scene01' sprite='resources://Medama/Pictures/20100818-1' layout='StretchStretch' top='8' bottom='8' left='8' right='8' />
  <AddNode name='Scene02' sprite='resources://Medama/Pictures/20100817-2' layout='StretchStretch' top='8' bottom='8' left='8' right='8' />
  <AddNode name='Scene03' sprite='resources://Medama/Pictures/20071109-5' layout='StretchStretch' top='8' bottom='8' left='8' right='8' />
  <AddNode name='Scene04' sprite='resources://Medama/Pictures/20071113-1' layout='StretchStretch' top='8' bottom='8' left='8' right='8' />
  <AddNode name='Scene05' sprite='resources://Medama/Pictures/20071201-1' layout='StretchStretch' top='8' bottom='8' left='8' right='8' />
  <AddNode name='Scene06' sprite='resources://Medama/Pictures/20080104-1' layout='StretchStretch' top='8' bottom='8' left='8' right='8' />
  <AddNode name='Scene07' sprite='resources://Medama/Pictures/20080104-2' layout='StretchStretch' top='8' bottom='8' left='8' right='8' />
  <AddNode name='Scene08' sprite='resources://Medama/Pictures/20080201-4' layout='StretchStretch' top='8' bottom='8' left='8' right='8' />
  <AddNode name='Scene09' sprite='resources://Medama/Pictures/20080923-9' layout='StretchStretch' top='8' bottom='8' left='8' right='8' />
  <AddNode name='Scene10' sprite='resources://Medama/Pictures/20081024-1' layout='StretchStretch' top='8' bottom='8' left='8' right='8' />
  <AddNode name='Scene11' sprite='resources://Medama/Pictures/20081119-1' layout='StretchStretch' top='8' bottom='8' left='8' right='8' />
  <AddNode name='Scene12' sprite='resources://Medama/Pictures/20090225-3' layout='StretchStretch' top='8' bottom='8' left='8' right='8' />
  <AddNode name='Scene13' sprite='resources://Medama/Pictures/20090605-1' layout='StretchStretch' top='8' bottom='8' left='8' right='8' />
  <AddNode name='Scene14' sprite='resources://Medama/Pictures/20100105-1' layout='StretchStretch' top='8' bottom='8' left='8' right='8' />
  <AddNode name='Scene15' sprite='resources://Medama/Pictures/20100126-1' layout='StretchStretch' top='8' bottom='8' left='8' right='8' />
  <AddNode name='Scene16' sprite='resources://Medama/Pictures/20100224-1' layout='StretchStretch' top='8' bottom='8' left='8' right='8' />
  <AddNode name='Scene17' sprite='resources://Medama/Pictures/20100805-1' layout='StretchStretch' top='8' bottom='8' left='8' right='8' />
  <AddNode name='Scene18' sprite='resources://Medama/Pictures/20100813-2' layout='StretchStretch' top='8' bottom='8' left='8' right='8' />
  <AddNode name='Scene19' sprite='resources://Medama/Pictures/20110428-1' layout='StretchStretch' top='8' bottom='8' left='8' right='8' />
  <AddNode name='Scene20' sprite='resources://Medama/Pictures/20110429-2' layout='StretchStretch' top='8' bottom='8' left='8' right='8' />
  <AddNode name='Scene21' sprite='resources://Medama/Pictures/20110623-1' layout='StretchStretch' top='8' bottom='8' left='8' right='8' />
  <AddNode name='Scene22' sprite='resources://Medama/Pictures/20110629-1' layout='StretchStretch' top='8' bottom='8' left='8' right='8' />
  <AddNode name='Scene23' sprite='resources://Medama/Pictures/20130604-02' layout='StretchStretch' top='8' bottom='8' left='8' right='8' />

  <!-- Buttons -->
  <AddNode name='Prev' sprite='resources://Medama/EUGML/UI001#Button001' layout='BottomLeft' left='16' bottom='16' width='64' height='32'>
    <SetButton textButton='PREV' colorButton='white' />
  </AddNode>
  <AddNode name='Next' sprite='resources://Medama/EUGML/UI001#Button001' layout='BottomLeft' left='88' bottom='16' width='64' height='32'>
    <SetButton textButton='NEXT' colorButton='white' />
  </AddNode>
</uGUI>");

        var scenes = dc.Where(p => p.Value.name.IndexOf("Scene") == 0).Select(p => { p.Value.SetActive(false); return p.Value; }).ToArray();
        var index = 0;
        var prev = dc.Where(p => p.Value.name == "Prev").First().Value.GetComponent<Button>();
        var next = dc.Where(p => p.Value.name == "Next").First().Value.GetComponent<Button>();
        scenes[index].SetActive(true);

        prev
            .OnClickAsObservable()
            .Subscribe(_ => {
                scenes[index].SetActive(false);
                index = (index == 0) ? (scenes.Length - 1) : (index - 1);
                scenes[index].SetActive(true);
            })
            .AddTo(this);

        next
            .OnClickAsObservable()
            .Subscribe(_ => {
                scenes[index].SetActive(false);
                index = (index + 1) % scenes.Length;
                scenes[index].SetActive(true);
            })
            .AddTo(this);

    }
}
