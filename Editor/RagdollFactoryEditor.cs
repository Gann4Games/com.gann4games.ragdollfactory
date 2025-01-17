#if UNITY_EDITOR
using System.IO;
using Gann4Games.RagdollFactory.States;
using UnityEditor;
using UnityEngine;

namespace Gann4Games.RagdollFactory
{
    [CustomEditor(typeof(RagdollFactory))]
    public partial class RagdollFactoryEditor : Editor
    {
        private static readonly GUIStyle GUIAlertStyle = new GUIStyle();
        private RagdollFactory _target;

        #region Serialized Properties

        // Capsule settings
        private SerializedProperty _capsuleRadius;
        private SerializedProperty _capsuleLength;
        // Box settings
        private SerializedProperty _boxWidth;
        private SerializedProperty _boxDepth;
        private SerializedProperty _boxLength;
        // Joint settings
        private SerializedProperty _jointAxis;
        private SerializedProperty _jointLowXLimit;
        private SerializedProperty _jointHighXLimit;
        private SerializedProperty _jointYLimit;
        private SerializedProperty _jointZAngle;
        // Rigidbody settings
        private SerializedProperty _rigidbodyMass;
        private SerializedProperty _rigidbodyDrag;
        private SerializedProperty _rigidbodyAngularDrag;
        private SerializedProperty _rigidbodyUseGravity;
        private SerializedProperty _rigidbodyIsKinematic;

        #endregion

        #region Icon paths
        private static string ContentPath => Path.Combine("Packages", "com.gann4games.ragdollfactory", "Content");
        private static string iconAddPath => Path.Combine(ContentPath, "Icons/Actions/add.png");
        private static string iconSelectPath => Path.Combine(ContentPath, "Icons/Actions/select.png");
        private static string iconDeletePath => Path.Combine(ContentPath, "Icons/Actions/delete.png");
        private static string iconCapsulePath => Path.Combine(ContentPath, "Icons/Components/capsule.png");
        private static string iconBoxPath => Path.Combine(ContentPath, "Icons/Components/box.png");
        private static string iconJointPath => Path.Combine(ContentPath, "Icons/Components/joint.png");
        private static string iconRigidbodyPath => Path.Combine(ContentPath, "Icons/Components/rigidbody.png");

        private Texture2D _add, _select, _delete, _capsule, _box, _joint, _rigidbody;
        #endregion
        
        private void OnEnable()
        {
            _target = (RagdollFactory)target;
            
            GUIAlertStyle.fontStyle = FontStyle.Bold;
            GUIAlertStyle.normal.textColor = Color.red;

            ApplySerializedProperties();
            LoadButtonIcons();
        }

        private void LoadButtonIcons()
        {
            _add = EditorGUIUtility.Load(iconAddPath) as Texture2D;
            _select = EditorGUIUtility.Load(iconSelectPath) as Texture2D;
            _delete = EditorGUIUtility.Load(iconDeletePath) as Texture2D;
            _capsule = EditorGUIUtility.Load(iconCapsulePath) as Texture2D;
            _box = EditorGUIUtility.Load(iconBoxPath) as Texture2D;
            _joint = EditorGUIUtility.Load(iconJointPath) as Texture2D;
            _rigidbody = EditorGUIUtility.Load(iconRigidbodyPath) as Texture2D;
        }

        private void ApplySerializedProperties()
        {
            _capsuleRadius = serializedObject.FindProperty("capsuleColliderRadius");
            _capsuleLength = serializedObject.FindProperty("capsuleColliderLength");

            _boxWidth = serializedObject.FindProperty("boxColliderWidth");
            _boxDepth = serializedObject.FindProperty("boxColliderDepth");
            _boxLength = serializedObject.FindProperty("boxColliderLength");

            _jointAxis = serializedObject.FindProperty("jointAxis");
            _jointLowXLimit = serializedObject.FindProperty("jointLowXLimit");
            _jointHighXLimit = serializedObject.FindProperty("jointHighXLimit");
            _jointYLimit = serializedObject.FindProperty("jointYLimit");
            _jointZAngle = serializedObject.FindProperty("jointZLimit");
            
            _rigidbodyMass = serializedObject.FindProperty("rigidbodyMass");
            _rigidbodyDrag = serializedObject.FindProperty("rigidbodyDrag");
            _rigidbodyAngularDrag = serializedObject.FindProperty("rigidbodyAngularDrag");
            _rigidbodyUseGravity = serializedObject.FindProperty("rigidbodyUseGravity");
            _rigidbodyIsKinematic = serializedObject.FindProperty("rigidbodyIsKinematic");
        }

        // Draw inspector
        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();
            DrawHelpBox();
            serializedObject.Update();
            DrawInspectorTabs();
            DrawCurrentComponentProperties();
            DrawActionButtons();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInspectorTabs()
        {
            GUIContent[] componentTabs = {
                new(_capsule, "Capsule colliders tab"),
                new(_box, "Box colliders tab"),
                new(_joint, "Configurable Joints tab"),
                new(_rigidbody, "Rigidbodies tab")
            };
            
            GUIContent[] actionTypeTabs = {
                new(_add, "Create"),
                new(_select, "Select"),
                new(_delete, "Delete")
            };
            
             _target.componentType = (RagdollFactory.ComponentType)GUILayout.Toolbar(_target.CurrentStateIndex(), componentTabs);
             _target.SetState(_target.States[(int)_target.componentType]);
            _target.actionTypeOnClick = (RagdollFactory.ActionTypeOnClick)GUILayout.Toolbar((int)_target.actionTypeOnClick, actionTypeTabs);
            
            GUILayout.Space(15);
        }
        
        #region Property Drawing
        private void DrawCurrentComponentProperties()
        {
            if(!_target.CurrentComponent.HasComponentSelected) return;
            switch (_target.componentType)
            {
                case RagdollFactory.ComponentType.Capsule:
                    DrawCapsuleColliderProperties();
                    break;
                case RagdollFactory.ComponentType.Box:
                    DrawBoxColliderProperties();
                    break;
                case RagdollFactory.ComponentType.ConfigurableJoint:
                    DrawJointProperties();
                    break;
                case RagdollFactory.ComponentType.Rigidbody:
                    DrawRigidbodyProperties();
                    break;
            }
        }
        private void DrawRigidbodyProperties()
        {
            // if(!(_target.CurrentComponent is RigidbodyComponentState)) return;
            
            EditorGUILayout.PropertyField(_rigidbodyMass);
            EditorGUILayout.PropertyField(_rigidbodyDrag);
            EditorGUILayout.PropertyField(_rigidbodyAngularDrag);
            EditorGUILayout.PropertyField(_rigidbodyUseGravity);
            EditorGUILayout.PropertyField(_rigidbodyIsKinematic);
        }
        private void DrawJointProperties()
        {
            // if(!(_target.CurrentComponent is ConfigurableJointComponentState)) return;
            
            EditorGUILayout.PropertyField(_jointAxis);
            EditorGUILayout.PropertyField(_jointLowXLimit);
            EditorGUILayout.PropertyField(_jointHighXLimit);
            EditorGUILayout.PropertyField(_jointYLimit);
            EditorGUILayout.PropertyField(_jointZAngle);
        }
        private void DrawCapsuleColliderProperties()
        {
            // if(!(_target.CurrentComponent is CapsuleColliderComponentState)) return;
            
            EditorGUILayout.PropertyField(_capsuleLength);
            EditorGUILayout.PropertyField(_capsuleRadius);
        }
        private void DrawBoxColliderProperties()
        {
            // if(!(_target.CurrentComponent is BoxColliderComponentState)) return;
            
            EditorGUILayout.PropertyField(_boxLength);
            EditorGUILayout.PropertyField(_boxWidth);
            EditorGUILayout.PropertyField(_boxDepth);
        }
        #endregion

        private void DrawActionButtons()
        {
            bool hasSelection = _target.CurrentComponent.HasComponentSelected;
            bool isCapsule = _target.CurrentComponent is CapsuleColliderComponentState && hasSelection;
            bool isBox = _target.CurrentComponent is BoxColliderComponentState && hasSelection;
            
            if (isBox && GUILayout.Button("Convert to Capsule Collider"))
                _target.CurrentComponent.ConvertTo(new CapsuleCollider());//.ConvertSelectedColliderToCapsule();
            if (isCapsule && GUILayout.Button("Convert to Box Collider"))
                _target.CurrentComponent.ConvertTo(new BoxCollider());//.ConvertSelectedColliderToBox();

            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Delete All"))
                _target.CurrentComponent.DeleteAll();
            if (hasSelection && GUILayout.Button("Delete Selected"))
                _target.CurrentComponent.Delete();
            EditorGUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// Shows relevant information about how to perfom a specific action in the choosen tool/button/mode
        /// </summary>
        private void DrawHelpBox()
        {
            string message = "";
            MessageType messageType = MessageType.Info;
            switch (_target.componentType)
            {
                case RagdollFactory.ComponentType.Capsule:
                    if (_target.actionTypeOnClick == RagdollFactory.ActionTypeOnClick.Create)
                        message += _target.IsFirstBoneSelected
                            ? "Now select the end point for your collider."
                            : "Select the bone that's going to have a collider.";
                    else if(_target.actionTypeOnClick == RagdollFactory.ActionTypeOnClick.Select)
                        message += "Select a collider in the scene view with left click to begin editing its properties.";
                    else if (_target.actionTypeOnClick == RagdollFactory.ActionTypeOnClick.Delete)
                        message += "Select a collider in the scene view with left click to delete it.";
                    break;
                case RagdollFactory.ComponentType.Box:
                    if (_target.actionTypeOnClick == RagdollFactory.ActionTypeOnClick.Create)
                        message += _target.IsFirstBoneSelected
                            ? "Now select the end point for your collider."
                            : "Select the bone that's going to have a collider.";
                    else if(_target.actionTypeOnClick == RagdollFactory.ActionTypeOnClick.Select)
                        message += "Select a collider in the scene view with left click to begin editing its properties.";
                    else if (_target.actionTypeOnClick == RagdollFactory.ActionTypeOnClick.Delete)
                        message += "Select a collider in the scene view with left click to delete it.";
                    
                    break;
                case RagdollFactory.ComponentType.ConfigurableJoint:
                    if(_target.actionTypeOnClick == RagdollFactory.ActionTypeOnClick.Create)
                        message += _target.IsFirstBoneSelected 
                            ? "Select the bone that is going to have the joint connecting to the first bone."
                            : "Select the bone that is going to be the parent of the joint you want to create.";
                    else if (_target.actionTypeOnClick == RagdollFactory.ActionTypeOnClick.Select)
                        message += "Select a joint in the scene view with left click to edit its properties.";
                    else if (_target.actionTypeOnClick == RagdollFactory.ActionTypeOnClick.Delete)
                        message += "Select a join in the scene view with Left Click to delete it.";
                    
                    break;
                case RagdollFactory.ComponentType.Rigidbody:
                    // messageType = MessageType.Warning;
                    if (_target.actionTypeOnClick == RagdollFactory.ActionTypeOnClick.Create)
                        message += "Click on a bone to create a rigidbody on it.";
                    else if (_target.actionTypeOnClick == RagdollFactory.ActionTypeOnClick.Select)
                        message += "Click on a rigidbody to edit its properties.";
                    else if (_target.actionTypeOnClick == RagdollFactory.ActionTypeOnClick.Delete)
                        message += "Click on a rigidbody delete it.";
                    break;
            }
            EditorGUILayout.HelpBox(message, messageType);
        }
    }
}
#endif