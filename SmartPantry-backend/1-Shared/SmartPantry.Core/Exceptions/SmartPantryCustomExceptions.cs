namespace SmartPantry.Core.Exceptions
{
    /// <summary>
    /// Base custom exception for all SmartPantry-specific business logic errors.
    /// </summary>
    public class SmartPantryException : Exception
    {
        public SmartPantryException() { }

        public SmartPantryException(string message)
            : base(message) { }

        public SmartPantryException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Thrown when attempting to create a user that already exists in the system,
    /// based on a unique constraint such as email address.
    /// </summary>
    public class UserAlreadyExistsException : SmartPantryException
    {
        public UserAlreadyExistsException(string email)
            : base($"A user with email '{email}' already exists.") { }
    }

    /// <summary>
    /// Thrown when a requested operation is not permitted in the current context,
    /// such as unauthorized updates or invalid state transitions.
    /// </summary>
    public class OperationNotAllowedException : SmartPantryException
    {
        public OperationNotAllowedException(string message)
            : base(message) { }
    }

    /// <summary>
    /// Thrown when input parameters provided to a method are invalid,
    /// such as null DTOs, empty GUIDs, or logically incorrect values.
    /// </summary>
    public class InvalidInputException : SmartPantryException
    {
        public InvalidInputException(string message)
            : base(message) { }
    }

    /// <summary>
    /// Thrown when a failure occurs during database persistence or retrieval,
    /// such as a failed insert, update, or query execution.
    /// Wraps lower-level exceptions to abstract data layer issues.
    /// </summary>
    public class PersistenceException : SmartPantryException
    {
        public PersistenceException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Thrown when a call to an external API fails due to network, API, or unexpected response structure.
    /// </summary>
    public class ExternalServiceException : SmartPantryException
    {
        public ExternalServiceException(string message, Exception? inner = null)
            : base(message, inner) { }
    }
}
