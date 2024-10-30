namespace ProductManager.Shared.DTOs;

public interface IMementoDto
{
    void SaveState();
    void RestoreState();
    void ClearState();
}
