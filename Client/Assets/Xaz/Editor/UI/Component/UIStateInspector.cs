//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Xaz;
using XazEditor;

[CustomEditor (typeof(UIState))]
public class UIStateInspector :Editor
{
 
	private UIState uiState = null;

	void OnEnable ()
	{
		if (target) {
			uiState = target as UIState;
			uiState.ResetDefaultValue ();
		}
	}

	public override void OnInspectorGUI ()
	{
		if (!Application.isPlaying) {
			OnInitGUI ();
			GUILayout.BeginHorizontal ();
			int oldIndex = uiState.index;
			if (uiState.states.Count > uiState.index) {
				uiState.index = XazEditorTools.DrawPrefixList (uiState.index, uiState.states.ConvertAll<string> (s => s.name).ToArray (), GUILayout.MinWidth (80));
				uiState.states [uiState.index].name = EditorGUILayout.TextField (uiState.states [uiState.index].name, GUILayout.MinWidth (80));
			}
			if (GUILayout.Button ("新增")) {
				////第一次新增两个状态
				//if (uiState.states.Count == 0) {
				//	uiState.states.Add (new UIState.State (){ name = uiState.states.Count.ToString () });
				//}
				uiState.states.Add (new UIState.State (){ name = uiState.states.Count.ToString () });
			}
			if (GUILayout.Button ("删除")) {
				if (uiState.states.Count > uiState.index) {
					uiState.states.RemoveAt (uiState.index);
					uiState.index = 0;
					uiState.ResetDefaultValue ();
				}
			}
			GUILayout.EndHorizontal ();
            //modifyby xiejie 2023-3-3
            if (uiState.states.Count > 0)
            {
                GUILayout.Space(10);
                if (GUILayout.Button(string.Format("复制状态【{0}】所有操作对象并新建State", uiState.states[0].name)))
                {
                    UIState.State nt = new UIState.State() { name = uiState.states.Count.ToString() };
                    DeepCopyNode(nt, uiState.states[0]);
                    uiState.states.Add(nt);
                }
                GUILayout.Space(8);
                if (GUILayout.Button("刷新当前状态"))
                {
                    uiState.gameObject.SetActive(false);
                    uiState.gameObject.SetActive(true);
                }
            }
            OnComponentListGUI ();
			if (uiState.index != oldIndex) {
				OnSwitchState (uiState.index);
			}

			if (GUI.changed) {
				Dirty ();
			}
		}
	}

    //jietodo 太麻烦待优化
    //状态复制 addby xiejie 2023-3-3
    public static UIState.State DeepCopyNode(UIState.State nt, UIState.State current)
    {
        int i = 0, count = 0;
        count = current.nodes.Count;
        for (i = 0; i < count; i++)
        {
            nt.nodes.Insert(i, new StateNode());
            nt.nodes[i].node = current.nodes[i].GetComponent();
        }
        count = current.rectTransforms.Count;
        for (i = 0; i < count; i++)
        {
            nt.rectTransforms.Insert(i, new StateRectTransform());
            nt.rectTransforms[i].rectTransform = current.rectTransforms[i].rectTransform;
        }
        count = current.uiImages.Count;
        for (i = 0; i < count; i++)
        {
            nt.uiImages.Insert(i, new StateImage());
            nt.uiImages[i].image = current.uiImages[i].image;
        }
        count = current.uiRawImages.Count;
        for (i = 0; i < count; i++)
        {
            nt.uiRawImages.Insert(i, new StateRawImage());
            nt.uiRawImages[i].rawImage = current.uiRawImages[i].rawImage;
        }
        count = current.uiTexts.Count;
        for (i = 0; i < count; i++)
        {
            nt.uiTexts.Insert(i, new StateText());
            nt.uiTexts[i].text = current.uiTexts[i].text;
        }
        count = current.uiOutLine.Count;
        for (i = 0; i < count; i++)
        {
            nt.uiOutLine.Insert(i, new StateOutline());
            nt.uiOutLine[i].outLine = current.uiOutLine[i].outLine;
        }
        count = current.uiGradient.Count;
        for (i = 0; i < count; i++)
        {
            nt.uiGradient.Insert(i, new StateGradient());
            nt.uiGradient[i].gradient = current.uiGradient[i].gradient;
        }
        count = current.uiShadow.Count;
        for (i = 0; i < count; i++)
        {
            nt.uiShadow.Insert(i, new StateShadow());
            nt.uiShadow[i].shadow = current.uiShadow[i].shadow;
        }
        count = current.uiGray.Count;
        for (i = 0; i < count; i++)
        {
            nt.uiGray.Insert(i, new StateGray());
            nt.uiGray[i].grayctrl = current.uiGray[i].grayctrl;
        }
        return nt;
    }
    //利用反射实现深度复制 jietodod 2023-3-3
    //public static T DeepCopy<T>(T obj)
    //{
    //    //如果是字符串或值类型则直接返回
    //    if (obj is string || obj.GetType().IsValueType) return obj;

    //    object retval = Activator.CreateInstance(obj.GetType());
    //    FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
    //    foreach (FieldInfo field in fields)
    //    {
    //        try { field.SetValue(retval, DeepCopy(field.GetValue(obj))); }
    //        catch { }
    //    }
    //    return (T)retval;
    //}

    GUIStyle dragBoxStyle;
	void OnInitGUI ()
	{
		if (dragBoxStyle == null) {
			dragBoxStyle = new GUIStyle (GUI.skin.box);
			dragBoxStyle.normal.textColor = Color.white;
		}
	}

	void OnSelectMenu (UIState.State state)
	{
		var options = new GUIContent[] {
			new GUIContent ("Node"), 
			new GUIContent ("RectTransform"), 
			new GUIContent ("Image"),
			new GUIContent ("RawImage"),
			new GUIContent ("Text"),
            new GUIContent ("Gray"),
            new GUIContent ("OutLine"),
			new GUIContent ("Gradient"),
			new GUIContent ("Shadow")
		};
		var current = Event.current;
		var mousePosition = current.mousePosition;
		var width = options.Length * 10;
		var height = 100;
		var position = new Rect (mousePosition.x, mousePosition.y - height, width, height);
		var selected = -1;
		EditorUtility.DisplayCustomMenu (position, options, selected, delegate(object callBackUserData, string[] callBackOptions, int callBackSelected) {
			UIState.State callBackState = callBackUserData as  UIState.State;
			switch (callBackOptions [callBackSelected]) {
			case "Node":
				callBackState.nodes.Add (new StateNode ());
				AddDefaultValue(this.uiState.defaults, callBackState.nodes [callBackState.nodes.Count - 1]);
				break;
			case "RectTransform":
				callBackState.rectTransforms.Add (new StateRectTransform ());
				AddDefaultValue(this.uiState.defaults, callBackState.rectTransforms [callBackState.rectTransforms.Count - 1]);
				break;
			case "Image":
				callBackState.uiImages.Add (new StateImage ());
				AddDefaultValue(this.uiState.defaults, callBackState.uiImages [callBackState.uiImages.Count - 1]);
				break;
			case "RawImage":
				callBackState.uiRawImages.Add (new StateRawImage ());
				AddDefaultValue(this.uiState.defaults, callBackState.uiRawImages [callBackState.uiRawImages.Count - 1]);
				break;
			case "Text":
				callBackState.uiTexts.Add (new StateText ());
				AddDefaultValue(this.uiState.defaults, callBackState.uiTexts [callBackState.uiTexts.Count - 1]);
				break;
             case "Gray":
                callBackState.uiGray.Add(new StateGray());
                AddDefaultValue(this.uiState.defaults, callBackState.uiGray[callBackState.uiGray.Count - 1]);
               break;
                case "OutLine":
				callBackState.uiOutLine.Add (new StateOutline ());
				AddDefaultValue(this.uiState.defaults, callBackState.uiOutLine [callBackState.uiOutLine.Count - 1]);
				break;
			case "Gradient":
				callBackState.uiGradient.Add (new StateGradient ());
				AddDefaultValue(this.uiState.defaults, callBackState.uiGradient [callBackState.uiGradient.Count - 1]);
				break; 
			case "Shadow":
				callBackState.uiShadow.Add (new StateShadow ());
				AddDefaultValue(this.uiState.defaults, callBackState.uiShadow [callBackState.uiShadow.Count - 1]);
				break;
			}
           
		}, state);

	}


	void OnComponentListGUI ()
	{

		int index = serializedObject.FindProperty ("index").intValue;

		List<UIState.State> states = uiState.states;
		if (states.Count > index) {
			UIState.State state = states [index];

			if (GUILayout.Button ("增加控制组件", GUILayout.Width (150))) {
				OnSelectMenu (state);
				EditorGUIUtility.ExitGUI ();
			}
			OnNode(state);
			OnRectTransform (state);
			OnImage (state);
			OnRawImage (state);
			OnText (state);
			OnOutLine (state);
			OnGradient (state);
			OnShadow (state);
			OnGrayShow(state);
        }
	}

	void OnNode(UIState.State state)
	{
		string titleNmae = "Node";
		for (int i = 0; i < state.nodes.Count; i++) {
			StateNode stateNode = state.nodes [i];
			if (XazEditorTools.DrawHeader (titleNmae, string.Format ("{0}_{1}_{2}", titleNmae, uiState.name, i))) {
				XazEditorTools.BeginContents ();
				GUILayout.BeginHorizontal ();
				stateNode.node = OnAddObjectField<Component> (titleNmae, stateNode.node);

				if (GUILayout.Button ("remove", GUILayout.Width (50))) {
					state.nodes.RemoveAt (i);
					this.uiState.ResetDefaultValue ();
					continue;
				}
				GUILayout.EndHorizontal ();
				if (stateNode.node) {
					stateNode.enable = GUILayout.Toggle (stateNode.enable, "enable");
					stateNode.node.gameObject.SetActive (stateNode.enable);
				}
               
				XazEditorTools.EndContents ();

				stateNode.CopyDataToCom ();
			}
		}
	}

	void OnRectTransform (UIState.State state)
	{
		string titleNmae = "RectTransform";
		for (int i = 0; i < state.rectTransforms.Count; i++) {
			StateRectTransform stateRectTransform = state.rectTransforms [i];
			if (XazEditorTools.DrawHeader (titleNmae, string.Format ("{0}_{1}_{2}", titleNmae, uiState.name, i))) {
				XazEditorTools.BeginContents ();
				GUILayout.BeginHorizontal ();
				stateRectTransform.rectTransform = OnAddObjectField<RectTransform> (titleNmae, stateRectTransform.rectTransform);

				if (GUILayout.Button ("remove", GUILayout.Width (50))) {
					state.rectTransforms.RemoveAt (i);
					this.uiState.ResetDefaultValue ();
					continue;
				}
				GUILayout.EndHorizontal ();
				if (stateRectTransform.rectTransform) {
					stateRectTransform.enable = GUILayout.Toggle (stateRectTransform.enable, "enable");
					stateRectTransform.rectTransform.gameObject.SetActive (stateRectTransform.enable);
					if (stateRectTransform.enable) {
						stateRectTransform.postion = EditorGUILayout.Vector3Field ("Position ", stateRectTransform.postion);
						stateRectTransform.widthAndHieght = EditorGUILayout.Vector2Field ("width&Hieght ", stateRectTransform.widthAndHieght);
						stateRectTransform.AnchorsMin = EditorGUILayout.Vector2Field ("AnchorsMin ", stateRectTransform.AnchorsMin);
						stateRectTransform.AnchorsMax = EditorGUILayout.Vector2Field ("AnchorsMax ", stateRectTransform.AnchorsMax);
						stateRectTransform.Pivot = EditorGUILayout.Vector2Field ("Pivot ", stateRectTransform.Pivot);
						stateRectTransform.rotation = EditorGUILayout.Vector3Field ("Rotation ", stateRectTransform.rotation);
						stateRectTransform.scale = EditorGUILayout.Vector3Field ("Scale ", stateRectTransform.scale);
					}
				}
               
				XazEditorTools.EndContents ();

				stateRectTransform.CopyDataToCom ();
			}
		}
	}

    void OnImage(UIState.State state)
    {
        string titleNmae = "Image";
        for (int i = 0; i < state.uiImages.Count; i++)
        {
            StateImage stateImage = state.uiImages[i];
            if (XazEditorTools.DrawHeader(titleNmae, string.Format("{0}_{1}_{2}", titleNmae, uiState.name, i)))
            {
                XazEditorTools.BeginContents();
                GUILayout.BeginHorizontal();
                //stateImage.image = EditorGUILayout.ObjectField(titleNmae, stateImage.image, typeof(Image)) as Image;
                stateImage.image = OnAddObjectField<Image>(titleNmae, stateImage.image);
                if (GUILayout.Button("remove", GUILayout.Width(50)))
                {
                    state.uiImages.RemoveAt(i);
                    this.uiState.ResetDefaultValue();
                    continue;
                }
                GUILayout.EndHorizontal();
                stateImage.codeImg = GUILayout.Toggle(stateImage.codeImg, "codeImg");
                if (stateImage.image)
                {
                    stateImage.enable = GUILayout.Toggle(stateImage.enable, "enable");
                    if (stateImage.enable)
                    {
                        if (!stateImage.codeImg)
                        {
                            stateImage.sprite = EditorGUILayout.ObjectField("sprite ", stateImage.sprite, typeof(Sprite)) as Sprite;
                            if (GUILayout.Button("set native size", GUILayout.Width(250)))
                            {
                                stateImage.image.SetNativeSize();
                                for (int j = 0; j < state.rectTransforms.Count; j++)
                                {
                                    StateRectTransform stateRectTransform = state.rectTransforms[j];
                                    if (stateRectTransform.rectTransform && stateRectTransform.rectTransform == stateImage.image.rectTransform)
                                    {
                                        stateRectTransform.widthAndHieght = stateImage.image.rectTransform.sizeDelta;
                                        break;
                                    }
                                }
                            }
                        }
                        stateImage.color = EditorGUILayout.ColorField("color ", stateImage.color);
                    }
                }
                XazEditorTools.EndContents();
                stateImage.CopyDataToCom();
            }
        }
    }

    void OnRawImage (UIState.State state)
	{
		string titleNmae = "RawImage";
		for (int i = 0; i < state.uiRawImages.Count; i++) {
			StateRawImage stateRawImage = state.uiRawImages [i];
			if (XazEditorTools.DrawHeader (titleNmae, string.Format ("{0}_{1}_{2}", titleNmae, uiState.name, i))) {
				XazEditorTools.BeginContents ();
				GUILayout.BeginHorizontal ();
				stateRawImage.rawImage = OnAddObjectField<RawImage> (titleNmae, stateRawImage.rawImage);
				if (GUILayout.Button ("remove", GUILayout.Width (50))) {
					state.uiRawImages.RemoveAt (i);
					this.uiState.ResetDefaultValue ();
					continue;
				}
				GUILayout.EndHorizontal ();
                stateRawImage.codeImg = GUILayout.Toggle(stateRawImage.codeImg, "codeImg");
                if (stateRawImage.rawImage) {
					stateRawImage.enable = GUILayout.Toggle (stateRawImage.enable, "enable");
					if (stateRawImage.enable) {
						if (!stateRawImage.codeImg)
						{
                            stateRawImage.texture = EditorGUILayout.ObjectField("texture ", stateRawImage.texture, typeof(Texture)) as Texture;
                        }
						stateRawImage.color = EditorGUILayout.ColorField ("color ", stateRawImage.color);
						if (GUILayout.Button ("set native size", GUILayout.Width (250))) {
							UIRawImageInspector.SetNativeSize (stateRawImage.rawImage);
							for (int j = 0; j < state.rectTransforms.Count; j++) {
								StateRectTransform stateRectTransform = state.rectTransforms [j];
								if (stateRectTransform.rectTransform && stateRectTransform.rectTransform == stateRawImage.rawImage.rectTransform) {
									stateRectTransform.widthAndHieght = stateRawImage.rawImage.rectTransform.sizeDelta;
									break;
								}
							}
						}
					}
				}
				XazEditorTools.EndContents ();
				stateRawImage.CopyDataToCom ();
			}
		}
	}


	void OnText (UIState.State state)
	{
		string titleName = "Text";
		for (int i = 0; i < state.uiTexts.Count; i++) {
			StateText stateText = state.uiTexts [i];
			if (XazEditorTools.DrawHeader (titleName, string.Format ("{0}_{1}_{2}", titleName, uiState.name, i))) {
				XazEditorTools.BeginContents ();
				GUILayout.BeginHorizontal ();
				stateText.text = OnAddObjectField<Text> (titleName, stateText.text);
				if (GUILayout.Button ("remove", GUILayout.Width (50))) {
					state.uiTexts.RemoveAt (i);
					this.uiState.ResetDefaultValue ();
					continue;
				}
				GUILayout.EndHorizontal ();
				if (stateText.text) {
					stateText.enable = GUILayout.Toggle (stateText.enable, "enable");
					stateText.text.gameObject.SetActive (stateText.enable);
					if (stateText.enable) {
						stateText.font = EditorGUILayout.ObjectField ("font ", stateText.font, typeof(Font)) as Font;
						stateText.fontSize = EditorGUILayout.IntField ("fontSize ", stateText.fontSize);
						//stateText.content = EditorGUILayout.TextField ("text ", stateText.content);
						stateText.color = EditorGUILayout.ColorField ("color ", stateText.color);
					}
				}
				XazEditorTools.EndContents ();
				stateText.CopyDataToCom ();
			}
		}
	}

	void OnOutLine (UIState.State state)
	{
		string titleName = "OutLine";
		for (int i = 0; i < state.uiOutLine.Count; i++) {
			StateOutline stateOutLine = state.uiOutLine [i];
			if (XazEditorTools.DrawHeader (titleName, string.Format ("{0}_{1}_{2}", titleName, uiState.name, i))) {
				XazEditorTools.BeginContents ();
				GUILayout.BeginHorizontal ();
				stateOutLine.outLine = OnAddObjectField<Outline> (titleName, stateOutLine.outLine);
				if (GUILayout.Button ("remove", GUILayout.Width (50))) {
					state.uiOutLine.RemoveAt (i);
					this.uiState.ResetDefaultValue ();
					continue;
				}
				GUILayout.EndHorizontal ();
				if (stateOutLine.outLine) {
					stateOutLine.enable = stateOutLine.outLine.enabled = GUILayout.Toggle (stateOutLine.enable, "enable");
					if (stateOutLine.enable) {
						stateOutLine.effectColor = EditorGUILayout.ColorField ("effectColor ", stateOutLine.effectColor);
						stateOutLine.effectDistance = EditorGUILayout.Vector2Field ("effectDistance ", stateOutLine.effectDistance);
					}
				}
				XazEditorTools.EndContents ();
				stateOutLine.CopyDataToCom ();
			}
		}
	}

	void OnGradient (UIState.State state)
	{
		string titleName = "Gradient";
		for (int i = 0; i < state.uiGradient.Count; i++) {
			StateGradient stateGradient = state.uiGradient [i];
			if (XazEditorTools.DrawHeader (titleName, string.Format ("{0}_{1}_{2}", titleName, uiState.name, i))) {
				XazEditorTools.BeginContents ();
				GUILayout.BeginHorizontal ();
				stateGradient.gradient = OnAddObjectField<UIGradient> (titleName, stateGradient.gradient);
				if (GUILayout.Button ("remove", GUILayout.Width (50))) {
					state.uiGradient.RemoveAt (i);
					this.uiState.ResetDefaultValue ();
					continue;
				}
				GUILayout.EndHorizontal ();
				if (stateGradient.gradient) {
					stateGradient.enable = stateGradient.gradient.enabled = GUILayout.Toggle (stateGradient.enable, "enable");
					if (stateGradient.enable) {
						stateGradient.topColor = EditorGUILayout.ColorField ("topColor ", stateGradient.topColor);
						stateGradient.bottomColor = EditorGUILayout.ColorField ("bottomColor ", stateGradient.bottomColor);
					}
				}
				XazEditorTools.EndContents ();
				stateGradient.CopyDataToCom ();
			}
		}
	}

	void OnShadow (UIState.State state)
	{
		string titleName = "Shadow";
		for (int i = 0; i < state.uiShadow.Count; i++) {
			StateShadow stateShadow = state.uiShadow [i];
			if (XazEditorTools.DrawHeader (titleName, string.Format ("{0}_{1}_{2}", titleName, uiState.name, i))) {
				XazEditorTools.BeginContents ();
				GUILayout.BeginHorizontal ();
				stateShadow.shadow = OnAddObjectField<Shadow> (titleName, stateShadow.shadow);
				if (GUILayout.Button ("remove", GUILayout.Width (50))) {
					state.uiShadow.RemoveAt (i);
					this.uiState.ResetDefaultValue ();
					continue;
				}
				GUILayout.EndHorizontal ();
				if (stateShadow.shadow) {
					stateShadow.enable = stateShadow.shadow.enabled = GUILayout.Toggle (stateShadow.enable, "enable");
					if (stateShadow.enable) {
						stateShadow.effectColor = EditorGUILayout.ColorField ("effectColor ", stateShadow.effectColor);
						stateShadow.effectDistance = EditorGUILayout.Vector2Field ("effectDistance ", stateShadow.effectDistance);
					}
				}
				XazEditorTools.EndContents ();
				stateShadow.CopyDataToCom ();
			}
		}
	}

    void OnGrayShow(UIState.State state)
    {
        string titleName = "GrayShow";
        for (int i = 0; i < state.uiGray.Count; i++)
        {
            StateGray stateGray = state.uiGray[i];
            if (XazEditorTools.DrawHeader(titleName, string.Format("{0}_{1}_{2}", titleName, uiState.name, i)))
            {
                XazEditorTools.BeginContents();
                GUILayout.BeginHorizontal();
                stateGray.grayctrl = OnAddObjectField<UIGray>(titleName, stateGray.grayctrl);
                if (GUILayout.Button("remove", GUILayout.Width(50)))
                {
                    state.uiGray.RemoveAt(i);
                    this.uiState.ResetDefaultValue();
                    continue;
                }
                GUILayout.EndHorizontal();
                if (stateGray.grayctrl)
                {
                    stateGray.isGray = EditorGUILayout.Toggle("isGray", stateGray.isGray);
                    stateGray.isCanSetRayCast = EditorGUILayout.Toggle("是否影响RaycastTarget", stateGray.isCanSetRayCast);
					if (stateGray.isCanSetRayCast)
					{
                        stateGray.disableSelect = EditorGUILayout.Toggle("置灰是否可点击", stateGray.disableSelect);
                    }
                }
                XazEditorTools.EndContents();
                stateGray.CopyDataToCom();
            }
        }
    }

    void OnSwitchState (int index)
	{
		if (uiState.states.Count > index) {
			UIState.State state = uiState.states [index];
			uiState.SetState (state.name);
		}
		Dirty ();
	}

	void AddDefaultValue (List<UIState.IState>defaults, UIState.IState state) 
	{
		if (!defaults.Contains (state)) {
			defaults.Add (state);
		}
	}

	T OnAddObjectField<T> (string name, T com) where T : Component
	{
		com = EditorGUILayout.ObjectField (name, com, typeof(T)) as T;
		if (com) {
			com.gameObject.hideFlags = HideFlags.None;
			foreach (Component c in  com.GetComponentsInChildren<Component>()) {
				//c.hideFlags = HideFlags.NotEditable;
			}
		}
		return com;
	}


	void Dirty ()
	{
        //EditorSceneManager.MarkAllScenesDirty();
        EditorUtility.SetDirty(uiState.gameObject);
		EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
        // 在下一帧执行保存和刷新操作,保证编辑状态下立即刷新  addby xiejie
        //if (uiState.gameObject.activeSelf)
        //{
        //    EditorApplication.update += () =>
        //    {
        //        if(uiState && uiState.gameObject)
        //        {
        //            uiState.gameObject.SetActive(false);
        //            uiState.gameObject.SetActive(true);
        //        }
        //        EditorApplication.update -= null;
        //    };
        //}
    }

	[MenuItem ("GameObject/UI/UIState/Create Empty",false,0)]
	static private void UIStateCreate ()
	{
		if (Selection.activeTransform) {
			GameObject go = new GameObject ("UIState", typeof(UIState), typeof(RectTransform));
            go.transform.tag = UIViewExporter.UIPropertyTagName;
            go.transform.SetParent (Selection.activeTransform,false);
			(go.transform as RectTransform).sizeDelta = Vector2.zero;
			Selection.activeGameObject = go;
		}
	}

    [MenuItem("GameObject/UI/UIState/Create Example", false, 0)]
    static public void UIStateCreateExample()
    {
        if (Selection.activeTransform)
        {
            Selection.activeGameObject = UIStateTools.CreateExample(Selection.activeTransform); 
        }
    }

    [MenuItem ("GameObject/UI/UIState/Revert", false, 0)]
    static private void UIStateRevert ()
	{
		foreach (GameObject go in Selection.gameObjects) {
			foreach (Transform t in  go.GetComponentsInChildren<Transform>()) {
				t.gameObject.hideFlags = HideFlags.None;
			}
		}
	}

}

public static  class UIStateTools
{
    static public GameObject CreateExample(Transform parent)
    {
        GameObject go = new GameObject("UIState", typeof(UIState), typeof(RectTransform));
        go.transform.SetParent(parent, false);
        UIState comp = go.GetComponent<UIState>();
        (go.transform as RectTransform).sizeDelta = Vector2.zero;
        GameObject img = new GameObject("img", typeof(Image), typeof(RectTransform));
        img.transform.SetParent(go.transform, false);
        comp.states.Clear();
        if (comp.states.Count == 0)
        {
            for (int i = 0; i < 2; i++)
            {
                comp.states.Add(new UIState.State() { name = i.ToString() });
                StateImage vt = new StateImage();
                vt.image = img.GetComponent<Image>();
                comp.states[i].uiImages.Add(vt);
                vt.color = i == 0 ? Color.black : Color.blue;
            }
        }
        return go;
    }
}

