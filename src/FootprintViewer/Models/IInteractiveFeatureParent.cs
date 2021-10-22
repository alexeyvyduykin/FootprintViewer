namespace FootprintViewer
{
    public interface IInteractiveFeatureParent
    {
        void OnStepCreating(IInteractiveFeature feature);

        void OnCreatingCompleted(IInteractiveFeature feature);

        void OnHoverCreating(IInteractiveFeature feature);
    }
}
