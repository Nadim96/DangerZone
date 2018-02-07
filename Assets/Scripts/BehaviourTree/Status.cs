namespace Assets.Scripts.BehaviourTree
{
    /// <summary>
    /// Used as status codes for BTNodes. Every tick returns a Status. 
    /// This status is used for the control flow of the behaviour tree.
    /// </summary>
    public enum Status
    {
        Success,
        Failure,
        Running,
        Invalid,
        Aborted
    }
}