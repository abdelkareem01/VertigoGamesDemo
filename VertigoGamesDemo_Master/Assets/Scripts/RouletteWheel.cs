using UnityEngine ;
using UnityEngine.UI ;
using DG.Tweening ;
using UnityEngine.Events ;
using System.Collections.Generic ;
using System.Collections;

    public class RouletteWheel : MonoBehaviour {

        
        [SerializeField] private Transform PickerWheelTransform;
        [SerializeField] private Transform wheelCircle;
        [SerializeField] private GameObject wheelSlicePrefab;
        [SerializeField] private Transform wheelSlicesParent;

        [Space]
        [Tooltip("Sounds :")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip tickAudioClip;
        [SerializeField][Range(0f, 1f)] private float volume = .5f;
        [SerializeField][Range(-3f, 3f)] private float pitch = 1f;

        [Space]
        [Tooltip("wheel settings :")]
        [Range(1, 20)] public int spinDuration = 3;
        [SerializeField][Range(.2f, 2f)] private float wheelSize = 1f;

        [Space]
        [Tooltip("wheel Slices :")]
        public List<WheelSlice> wheelSlices = new List<WheelSlice>();

        private float itemsDistRadius = 0;

        private UnityAction onSpinStartEvent;
        private UnityAction<WheelSlice> onSpinEndEvent;

        private Button button;

        private bool _isSpinning = false;

        private Vector2 SliceMinSize = new Vector2(81f, 146f);
        private Vector2 SliceMaxSize = new Vector2(144f, 213f);

        private float sliceAngle;
        private float halfSliceAngle;

        private int slicesMin = 1;
        private int slicesMax = 8;

        private double accumulatedWeight;
        private System.Random rand = new System.Random();

        private List<int> nonZeroChancesIndices = new List<int>();

        void Awake()
        {
            wheelSlicesParent = this.transform.GetChild(0).transform;
            setDistRad();
            Invoke("OnValidate", 0f);
        }
        void Start() {

            this.transform.localScale = Vector3.zero;
            this.GetComponent<RectTransform>().DOScale(wheelSize, 1.5f).SetEase(Ease.OutBack);
        }

        private void InitSound() {
            audioSource.clip = tickAudioClip;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
        }

        private void Generate() {
            wheelSlicePrefab = InstantiateSlice();

            RectTransform rt = wheelSlicePrefab.transform.GetChild(0).GetComponent<RectTransform>();
            float SliceWidth = Mathf.Lerp(SliceMinSize.x, SliceMaxSize.x, 1f - Mathf.InverseLerp(slicesMin, slicesMax, wheelSlices.Count));
            float SliceHeight = Mathf.Lerp(SliceMinSize.y, SliceMaxSize.y, 1f - Mathf.InverseLerp(slicesMin, slicesMax, wheelSlices.Count));
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, SliceWidth);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, SliceHeight);

            for (int i = 0; i < 8; i++)
                RenderSlice(i);

            wheelSlicePrefab.transform.GetChild(0).gameObject.SetActive(false);
        }

        private void RenderSlice(int index) {
            WheelSlice Slice = wheelSlices[index];
            Transform SliceTrns = InstantiateSlice().transform.GetChild(0);
            SliceTrns.GetChild(0).GetComponent<Image>().sprite = Slice.Icon;
            SliceTrns.GetChild(0).GetComponent<Image>().rectTransform.sizeDelta = Slice.resolution;
            SliceTrns.gameObject.name = Slice.Index.ToString();

            Vector3 _SliceTrns = itemsDistRadius * Vector3.Normalize(SliceTrns.transform.position - wheelSlicesParent.transform.position) + wheelSlicesParent.transform.position;
            float _angle = (sliceAngle) * index;
            SliceTrns.transform.position = _SliceTrns;
            SliceTrns.RotateAround(wheelSlicesParent.transform.position, Vector3.back, _angle);

            this.transform.GetChild(this.transform.childCount - 1).SetSiblingIndex(1);
        }


        private GameObject InstantiateSlice() {
            return Instantiate(wheelSlicePrefab, wheelSlicesParent.position, Quaternion.identity, wheelSlicesParent);
        }


        public void Spin() {
            if (!_isSpinning) {
                _isSpinning = true;
                if (onSpinStartEvent != null)
                    onSpinStartEvent.Invoke();

                int index = GetRandomSliceIndex();
                WheelSlice Slice = wheelSlices[index];

                if (Slice.Chance == 0 && nonZeroChancesIndices.Count != 0) {
                    index = nonZeroChancesIndices[Random.Range(0, nonZeroChancesIndices.Count)];
                    Slice = wheelSlices[index];
                }

                float angle = -((sliceAngle) * index);
            
                Vector3 targetRotation = Vector3.back * (angle + 2f * 360 * spinDuration);

                float prevAngle, currentAngle;
                prevAngle = currentAngle = wheelCircle.eulerAngles.z;

                bool isIndicatorOnTheLine = false;

                wheelCircle
                .DORotate(targetRotation, spinDuration, RotateMode.Fast)
                .SetEase(Ease.InOutQuart)
                .OnUpdate(() => {
                    float diff = Mathf.Abs(prevAngle - currentAngle);
                    if (diff >= halfSliceAngle) {
                        if (isIndicatorOnTheLine) {
                            audioSource.PlayOneShot(audioSource.clip);
                        }
                        prevAngle = currentAngle;
                        isIndicatorOnTheLine = !isIndicatorOnTheLine;
                    }
                    currentAngle = wheelCircle.eulerAngles.z;
                })
                .OnComplete(() => {

                    _isSpinning = false;
                    if (onSpinEndEvent != null)

                        onSpinEndEvent.Invoke(Slice);

                    onSpinStartEvent = null;
                    onSpinEndEvent = null;
                    Slice = null;
                });

            }
        }

        public void OnSpinStart(UnityAction action) {
            onSpinStartEvent = action;
        }

        public void OnSpinEnd(UnityAction<WheelSlice> action) {
            onSpinEndEvent = action;
        }


        private int GetRandomSliceIndex() {
            double r = rand.NextDouble() * accumulatedWeight;

            for (int i = 0; i < wheelSlices.Count; i++)
                if (wheelSlices[i]._weight >= r)
                    return i;

            return 0;
        }

        private void CalculateWeightsAndIndices() {
            for (int i = 0; i < wheelSlices.Count; i++) {
                WheelSlice Slice = wheelSlices[i];

                //add weights:
                accumulatedWeight += Slice.Chance;
                Slice._weight = accumulatedWeight;

                //add index :
                Slice.Index = i;

                //save non zero chance indices:
                if (Slice.Chance > 0)
                    nonZeroChancesIndices.Add(i);
            }
        }


        private void OnValidate() {

            button = this.transform.GetChild(2).GetComponent<Button>();
            button.onClick.AddListener(() => {

                button.interactable = false;

                this.OnSpinEnd(wheelSlice => {
                    GameObject.Find(wheelSlice.Index + "").transform.SetParent(GameObject.Find("ui_wheelCanvas").transform);
                    float wheelY = GameObject.Find(wheelSlice.Index + "").transform.position.y;
                    GameObject.Find(wheelSlice.Index + "").transform.DOMoveY(wheelY -  (itemsDistRadius), 1f).OnComplete(() =>
                    {
                        LevelManager.instance.obtainedItem = wheelSlice;
                        LevelManager.instance.loadItemMenu();
                        LevelManager.instance.level++;
                        LevelManager.instance.premiumLoaded = false;
                        LevelManager.instance.normalLoaded = false;
                        button.interactable = true;

                    });

                });

                this.Spin();

            });
        }

        public void setPool(List<WheelSlice> pool)
        {

            for (int i = 0; i < slicesMax; i++)
            {
                wheelSlices.Add(pool[i]);
            }

            sliceAngle = 360 / wheelSlices.Count;
            halfSliceAngle = sliceAngle / 2f;

            CalculateWeightsAndIndices();
            Generate();

            if (nonZeroChancesIndices.Count == 0)
                Debug.LogError("You can't set all Slices chance to zero");

            InitSound();
        }


        public void setDistRad() {

            string res = Display.main.systemWidth.ToString() + " " + Display.main.systemHeight.ToString();
            if (res.Contains("1280") && res.Contains("720"))
            {
                itemsDistRadius = 94f * wheelSize;
            }
            else if (res.Contains("1920") && res.Contains("1080"))
            {
                itemsDistRadius = 140f * wheelSize;
            }
            else if (res.Contains("800") && res.Contains("600"))
            {
                itemsDistRadius = 79f * wheelSize;
            }
            else if (res.Contains("800") && res.Contains("480"))
            {
                itemsDistRadius = 63f * wheelSize;
            }
            else if (!res.Contains("1919") && res.Contains("863.50"))
            {
                itemsDistRadius = 113f * wheelSize;
            }
            else if (res.Contains("1919") && res.Contains("863.50")) {

                itemsDistRadius = 140f * wheelSize;
            }

            else
            {
                this.gameObject.GetComponentInParent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                itemsDistRadius = 140f;
            }

        }
    }
