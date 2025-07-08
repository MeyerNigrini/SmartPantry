namespace SmartPantry.Core.Exceptions
{
    /// <summary>
    /// Base custom exception for all SmartPantry-specific business logic errors.
    /// </summary>
    public class SmartPantryException : Exception
    {
        public SmartPantryException() { }

        public SmartPantryException(string message)
            : base(message)
        {
        }

        public SmartPantryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when a user with the provided email already exists.
    /// </summary>
    public class UserAlreadyExistsException : SmartPantryException
    {
        public UserAlreadyExistsException(string email)
            : base($"A user with email '{email}' already exists.")
        {
        }
    }

    /// <summary>
    /// Exception thrown when an entity is not found in the database.
    /// </summary>
    public class EntityNotFoundException : SmartPantryException
    {
        public EntityNotFoundException(string entityName, Guid id)
            : base($"{entityName} with ID '{id}' was not found.")
        {
        }

        public EntityNotFoundException(string entityName, string identifier)
            : base($"{entityName} with identifier '{identifier}' was not found.")
        {
        }
    }

    /// <summary>
    /// Exception thrown when an operation is not allowed in the current context.
    /// </summary>
    public class OperationNotAllowedException : SmartPantryException
    {
        public OperationNotAllowedException(string message)
            : base(message)
        {
        }
    }
}
