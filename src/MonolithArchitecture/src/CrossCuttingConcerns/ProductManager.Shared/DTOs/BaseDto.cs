namespace ProductManager.Shared.DTOs;

public abstract class BaseDto : IMementoDto
{
    private object? _state;

    public void SaveState() => _state = this.DeepClone();

    public void RestoreState()
    {
        foreach (var property in GetType().GetProperties().Where(p => p.CanWrite))
        {
            property.SetValue(this, property.GetValue(_state, null), null);
        }
    }
    public void ClearState() => _state = null;
}
