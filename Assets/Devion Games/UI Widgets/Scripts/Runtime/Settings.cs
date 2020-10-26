using UnityEngine;
using System.Collections;
using System.Linq;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif


namespace DevionGames
{
	public class Settings : MonoBehaviour {
		private const string QUALITY_LEVEL_KEY = "QualityLevel";
		private const string RESOLUTION_WIDTH_KEY = "ScreenResolutionWidth";
		private const string RESOLUTION_HEIGHT_KEY = "ScreenResolutionHeight";
		private const string FULL_SCREEN_KEY = "Fullscreen";

		private void Start(){
			int qualityIndex = PlayerPrefs.GetInt (QUALITY_LEVEL_KEY, QualitySettings.GetQualityLevel ());
			SetQualityLevel (qualityIndex);

			int resolutionWidth = PlayerPrefs.GetInt (RESOLUTION_WIDTH_KEY, Screen.currentResolution.width);
			int resolutionHeight = PlayerPrefs.GetInt (RESOLUTION_HEIGHT_KEY, Screen.currentResolution.height);
			SetResolution (resolutionWidth, resolutionHeight);

			bool fullScreen = IntToBool(PlayerPrefs.GetInt (FULL_SCREEN_KEY, BoolToInt(Screen.fullScreen)));
			SetFullscreen (fullScreen);
		}

		#region Quality
		public string QualityContent {
			get {
				return QualitySettings.names [QualitySettings.GetQualityLevel ()];
			}
		}

        public void SetQualityLevel(float index)
        {
            SetQualityLevel(Mathf.RoundToInt(index));
        }

        public void SetQualityLevel(int index){
			if (QualitySettings.GetQualityLevel () != index) {
				QualitySettings.SetQualityLevel (index,true);
				PlayerPrefs.SetInt (QUALITY_LEVEL_KEY,index);
			}
		}

		public void IncreaseQualityLevel(){
			QualitySettings.IncreaseLevel ();
			PlayerPrefs.SetInt (QUALITY_LEVEL_KEY,QualitySettings.GetQualityLevel());
		}

		public void DecreaseQualityLevel(){
			QualitySettings.DecreaseLevel ();
			PlayerPrefs.SetInt (QUALITY_LEVEL_KEY,QualitySettings.GetQualityLevel());
		}
		#endregion

		#region Resolution
		public string ResolutionContent {
			get {
				Resolution current=Screen.currentResolution;
				return current.width.ToString()+"x"+current.height.ToString();
			}
		}

		public void SetResolution(int index){
			Resolution resolution = Screen.resolutions [index];
			SetResolution (resolution.width, resolution.height);
		}

		public void SetResolution(int width, int height){
			if (IsStandalone && Screen.fullScreen) {
				Screen.SetResolution (width, height, Screen.fullScreen);
				PlayerPrefs.SetInt (RESOLUTION_WIDTH_KEY, width);
				PlayerPrefs.SetInt (RESOLUTION_HEIGHT_KEY, height);
			}
		}

		public void IncreaseResolution(){
			for (int i=0; i< Screen.resolutions.Length; i++) {
				Resolution resolution=Screen.resolutions[i];
				if(resolution.width > Screen.currentResolution.width){
					SetResolution(i);
					break;
				}
			}
		}

		public void DecreaseResolution(){
			for (int i=0; i< Screen.resolutions.Length; i++) {
				Resolution resolution=Screen.resolutions[i];
				if(resolution.width == Screen.currentResolution.width && i >0){
					SetResolution(i-1);
					break;
				}
			}
		}
		#endregion

		#region Fullscreen
		public bool IsFullscreen{
			get{
				return Screen.fullScreen;
			}
		}

		public void SetFullscreen(bool fullScreen){
			Screen.fullScreen = fullScreen;
			PlayerPrefs.SetInt (FULL_SCREEN_KEY, BoolToInt(fullScreen));
		}
		#endregion

		#region Antialiasing
		public string AntialiasingContent{
			get{
				return QualitySettings.antiAliasing == 0?"Disabled":QualitySettings.antiAliasing.ToString()+"x";
			}
		}

		public void SetAntialiasing(int value){
			QualitySettings.antiAliasing = value;
		}

		public void IncreaseAntialiasing(){
			SetAntialiasing (Mathf.Max(QualitySettings.antiAliasing*2,2));
		}

		public void DecreaseAntialisasing(){
			SetAntialiasing (QualitySettings.antiAliasing/2);
		}
		#endregion

		#region TextureQuality
		public string TextureQualityContent{
			get{
				return System.Enum.GetName(typeof(TextureQuality),QualitySettings.masterTextureLimit);
			}
		}

		public void SetTextureQuality(int textureLimit){
			QualitySettings.masterTextureLimit = textureLimit;
		}

		public void IncreaseTextureQuality(){
			QualitySettings.masterTextureLimit -= 1;
		}

		public void DecreaseTextureQuality(){
			QualitySettings.masterTextureLimit = Mathf.Clamp(QualitySettings.masterTextureLimit+1,0,3);
		}
        #endregion

        public void SetEffectAntialiasing(bool state) {
#if UNITY_POST_PROCESSING_STACK_V2
            PostProcessLayer layer = Camera.main.GetComponent<PostProcessLayer>();
            if (layer == null) { return; }
            if (state) {
                layer.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
            }else{
                layer.antialiasingMode = PostProcessLayer.Antialiasing.None;
            }
#endif
        }

        public void SetEffectBloom(bool state)
        {
            #if UNITY_POST_PROCESSING_STACK_V2
            PostProcessVolume volume = Camera.main.GetComponent<PostProcessVolume>();
            if (volume == null) { return; }
            Bloom bloom;
            if (volume.profile.TryGetSettings<Bloom>(out bloom))
            {
                bloom.active = state;
            }
            #endif
        }

        public void SetEffectVignette(bool state)
        {
#if UNITY_POST_PROCESSING_STACK_V2
            PostProcessVolume volume = Camera.main.GetComponent<PostProcessVolume>();
            if (volume == null) { return; }
            Vignette vignette;
            if (volume.profile.TryGetSettings<Vignette>(out vignette))
            {
                vignette.active = state;
            }
#endif
        }

        public void SetEffectAmbient(bool state)
        {
#if UNITY_POST_PROCESSING_STACK_V2
            PostProcessVolume volume = Camera.main.GetComponent<PostProcessVolume>();
            if (volume == null) { return; }
            AmbientOcclusion ambient;
            if (volume.profile.TryGetSettings<AmbientOcclusion>(out ambient))
            {
                ambient.active = state;
            }
#endif
        }

        public void SetAllEffects(bool state)
        {
            SetEffectAntialiasing(state);
            SetEffectAmbient(state);
            SetEffectBloom(state);
            SetEffectVignette(state);
        }

        public bool IsStandalone{
			get{ 
#if UNITY_STANDALONE
				return true;
#else
				return false;
#endif
			}
		}

		private static int BoolToInt(bool value){
			return value ? 1 : 0;
		}

		private static bool IntToBool(int value){
			return value == 0 ? false : true;
		}

		public enum TextureQuality:int{
			Full=0,
			Half=1,
			Quarter=2,
			Eighth=3
		}
	}
}