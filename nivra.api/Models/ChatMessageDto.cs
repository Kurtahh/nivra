namespace backend.Models;

public record ChatMessageDto(
    int Id,
    DateTime DateT,
    string Message,
    long UserId,
    string Username
);