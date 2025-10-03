using Stateless;
namespace Modern.Services
{
	public enum UserState
	{
		NotRegistered,
		Registered,
		Blocked
	}

	public enum UserTrigger
	{
		Register,
		Block,
		Unblock,
		CompleteRegistration
	}

	public class User
	{
		private readonly StateMachine<UserState, UserTrigger> _stateMachine;

		// Properties required for registration
		public string? Email { get; private set; }
		public string? Name { get; private set; }
		public bool IsEmailConfirmed { get; private set; }

		public UserState State => _stateMachine.State;

		public User()
		{
			_stateMachine = new StateMachine<UserState, UserTrigger>(UserState.NotRegistered);

			ConfigureStateMachine();
		}

		private void ConfigureStateMachine()
		{
			_stateMachine.Configure(UserState.NotRegistered)
				.Permit(UserTrigger.CompleteRegistration, UserState.Registered)
				.Permit(UserTrigger.Block, UserState.Blocked);

			_stateMachine.Configure(UserState.Registered)
				.Permit(UserTrigger.Block, UserState.Blocked);

			_stateMachine.Configure(UserState.Blocked)
				.Permit(UserTrigger.Unblock, UserState.Registered);
		}

		// Internal method to transition state — could be guarded
		public void FireTrigger(UserTrigger trigger)
		{
			_stateMachine.Fire(trigger);
		}

		// Registration flow
		public bool TryCompleteRegistration(string email, string name, bool isEmailConfirmed, out RegisteredUser? registeredUser)
		{
			registeredUser = null;

			if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name) || !isEmailConfirmed)
				return false;

			Email = email;
			Name = name;
			IsEmailConfirmed = isEmailConfirmed;

			FireTrigger(UserTrigger.CompleteRegistration);

			registeredUser = new RegisteredUser(this);
			return true;
		}

		public void Block()
		{
			FireTrigger(UserTrigger.Block);
		}

		public BlockedUser Unblock()
		{
			FireTrigger(UserTrigger.Unblock);
			return new BlockedUser(this); // or return RegisteredUser?
		}


	}

	// Marker classes for type safety
	public abstract class UserInState
	{
		public User InnerUser { get; }

		protected UserInState(User user)
		{
			InnerUser = user;
		}
	}

	public class RegisteredUser : UserInState
	{
		public RegisteredUser(User user) : base(user)
		{
			if (user.State != UserState.Registered)
				throw new InvalidOperationException("User must be in Registered state.");
		}

		public void PerformSensitiveAction()
		{
			Console.WriteLine($"User {InnerUser.Name} performed a sensitive action.");
		}
	}

	public class BlockedUser : UserInState
	{
		public BlockedUser(User user) : base(user)
		{
			if (user.State != UserState.Blocked)
				throw new InvalidOperationException("User must be in Blocked state.");
		}

		public RegisteredUser Unblock()
		{
			InnerUser.FireTrigger(UserTrigger.Unblock);
			return new RegisteredUser(InnerUser);
		}
	}

	public class StateService
	{
		// Only callable with a RegisteredUser
		public void TransferMoney(RegisteredUser user, decimal amount)
		{
			Console.WriteLine($"{user.InnerUser.Name} transferred {amount:C}");
		}

		// Only callable with a BlockedUser
		public void ReactivateAccount(BlockedUser blockedUser)
		{
			var activeUser = blockedUser.Unblock();
			Console.WriteLine($"Account reactivated for {activeUser.InnerUser.Name}");
		}

		// Open method: accepts any user, but checks state
		public void SendMessage(User user)
		{
			if (user.State == UserState.Blocked)
			{
				throw new InvalidOperationException("Cannot send message to blocked user.");
			}

			Console.WriteLine($"Message sent to {user.Name}");
		}
	}
	/*
	public class Program
	{
		static void Main()
		{
			var user = new User();

			// ❌ Cannot call TransferMoney yet — not registered
			var userService = new StateService();

			// Try to complete registration
			if (user.TryCompleteRegistration(
				email: "alice@example.com",
				name: "Alice",
				isEmailConfirmed: true,
				out var registeredUser))
			{
				// ✅ Now we have a RegisteredUser
				userService.TransferMoney(registeredUser, 100.00m); // OK

				// Block the user
				user.Block();

				// ❌ Cannot use registeredUser anymore — it wraps old state
				// But we can create a BlockedUser wrapper
				try
				{
					userService.TransferMoney(registeredUser, 50);
				}
				catch (InvalidOperationException)
				{
					Console.WriteLine("Cannot use stale registered user after blocking.");
				}

				var blockedUser = new BlockedUser(user);
				userService.ReactivateAccount(blockedUser);
			}
			else
			{
				Console.WriteLine("Registration failed.");
			}
		}
	}
	*/
}
