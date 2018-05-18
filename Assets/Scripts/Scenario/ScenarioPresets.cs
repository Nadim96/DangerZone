using Assets.Scripts.BehaviourTree;
using Assets.Scripts.Scenario;
using Assets.Scripts.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenarioPresets : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] private string LevelName;
    [Header("Starter")]
    [SerializeField] private int StarterEnemiesMin;
    [SerializeField] private int StarterEnemiesMax;
    [SerializeField] private int StarterCivMin;
    [SerializeField] private int StarterCivMax;
    [SerializeField] private float StarterReactionSpeed;
    [SerializeField] private int StarterRoomSizeMin;
    [SerializeField] private int StarterRoomSizeMax;
    [SerializeField] private WeaponSize StarterWeaponSize;
    [SerializeField] private MovementType StarterMovementType;
    [SerializeField] private int StarterEngagmentDistance;
    [Header("Average")]
    [SerializeField] private int AverageEnemiesMin;
    [SerializeField] private int AverageEnemiesMax;
    [SerializeField] private int AverageCivMin;
    [SerializeField] private int AverageCivMax;
    [SerializeField] private float AverageReactionSpeed;
    [SerializeField] private int AverageRoomSizeMin;
    [SerializeField] private int AverageRoomSizeMax;
    [SerializeField] private WeaponSize AverageWeaponSize;
    [SerializeField] private MovementType AverageMovementType;
    [SerializeField] private int AverageEngagmentDistance;
    [Header("Advanced")]
    [SerializeField] private int AdvancedEnemiesMin;
    [SerializeField] private int AdvancedEnemiesMax;
    [SerializeField] private int AdvancedCivMin;
    [SerializeField] private int AdvancedCivMax;
    [SerializeField] private float AdvancedReactionSpeed;
    [SerializeField] private int AdvancedRoomSizeMin;
    [SerializeField] private int AdvancedRoomSizeMax;
    [SerializeField] private WeaponSize AdvancedWeaponSize;
    [SerializeField] private MovementType AdvancedMovementType;
    [SerializeField] private int AdvancedEngagmentDistance;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadStarterPreset()
    {
        ScenarioSettings.MinEnemies = this.StarterEnemiesMin;
        ScenarioSettings.MaxEnemies = this.StarterEnemiesMax;
        ScenarioSettings.MinFriendlies = this.StarterCivMin;
        ScenarioSettings.MaxFriendlies = this.StarterCivMax;
        ScenarioSettings.MinRoomSize = this.StarterRoomSizeMin;
        ScenarioSettings.MaxRoomSize = this.StarterRoomSizeMax;
        ScenarioSettings.WeaponSize = this.StarterWeaponSize;
        ScenarioSettings.ReactionTime = this.StarterReactionSpeed;
        ScenarioSettings.MovementType = this.StarterMovementType;
        ScenarioSettings.EngagementDistance = this.StarterEngagmentDistance;

        SceneManager.LoadSceneAsync(LevelName);
    }

    public void LoadAveragePreset()
    {
        ScenarioSettings.MinEnemies = this.AverageEnemiesMin;
        ScenarioSettings.MaxEnemies = this.AverageEnemiesMax;
        ScenarioSettings.MinFriendlies = this.AverageCivMin;
        ScenarioSettings.MaxFriendlies = this.AverageCivMax;
        ScenarioSettings.MinRoomSize = this.AverageRoomSizeMin;
        ScenarioSettings.MaxRoomSize = this.AverageRoomSizeMax;
        ScenarioSettings.WeaponSize = this.AverageWeaponSize;
        ScenarioSettings.ReactionTime = this.AverageReactionSpeed;
        ScenarioSettings.MovementType = this.AverageMovementType;
        ScenarioSettings.EngagementDistance = this.AverageEngagmentDistance;

        SceneManager.LoadSceneAsync(LevelName);
    }

    public void LoadAdvancedPreset()
    {
        ScenarioSettings.MinEnemies = this.AdvancedEnemiesMin;
        ScenarioSettings.MaxEnemies = this.AdvancedEnemiesMax;
        ScenarioSettings.MinFriendlies = this.AdvancedCivMin;
        ScenarioSettings.MaxFriendlies = this.AdvancedCivMax;
        ScenarioSettings.MinRoomSize = this.AdvancedRoomSizeMin;
        ScenarioSettings.MaxRoomSize = this.AdvancedRoomSizeMax;
        ScenarioSettings.WeaponSize = this.AdvancedWeaponSize;
        ScenarioSettings.ReactionTime = this.AdvancedReactionSpeed;
        ScenarioSettings.MovementType = this.AdvancedMovementType;
        ScenarioSettings.EngagementDistance = this.AdvancedEngagmentDistance;

        SceneManager.LoadSceneAsync(LevelName);
    }
}
