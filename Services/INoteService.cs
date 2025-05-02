using KitapOkumaAPI.Models;

namespace KitapOkumaAPI.Services
{
	public interface INoteService
	{
		Task<List<Note>> GetNotesAsync();
		Task<Note?> GetNoteByIdAsync(int id);
		Task<Note> AddNoteAsync(Note note);
		Task<bool> UpdateNoteAsync(Note note);
		Task<bool> DeleteNoteAsync(int id);
	}
}
