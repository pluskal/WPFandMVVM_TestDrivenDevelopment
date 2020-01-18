﻿using System;
using System.Linq;
using FriendStorage.Model;

namespace FriendStorage.UI.Wrapper
{
    public class FriendWrapper : ModelWrapper<Friend>
    {
        public FriendWrapper(Friend model) : base(model)
        {
            InitializeComplexProperties(model);
            InitializeCollectionProperties(model);
        }

        public int Id
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public int IdOriginalValue => GetOriginalValue<int>(nameof(Id));

        public bool IdIsChanged => GetIsChanged(nameof(Id));

        public int FriendGroupId
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public int FriendGroupIdOriginalValue => GetOriginalValue<int>(nameof(FriendGroupId));

        public bool FriendGroupIdIsChanged => GetIsChanged(nameof(FriendGroupId));

        public string FirstName
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string FirstNameOriginalValue => GetOriginalValue<string>(nameof(FirstName));

        public bool FirstNameIsChanged => GetIsChanged(nameof(FirstName));

        public string LastName
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string LastNameOriginalValue => GetOriginalValue<string>(nameof(LastName));

        public bool LastNameIsChanged => GetIsChanged(nameof(LastName));

        public DateTime? Birthday
        {
            get => GetValue<DateTime?>();
            set => SetValue(value);
        }

        public DateTime? BirthdayOriginalValue => GetOriginalValue<DateTime?>(nameof(Birthday));

        public bool BirthdayIsChanged => GetIsChanged(nameof(Birthday));

        public bool IsDeveloper
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        public bool IsDeveloperOriginalValue => GetOriginalValue<bool>(nameof(IsDeveloper));

        public bool IsDeveloperIsChanged => GetIsChanged(nameof(IsDeveloper));

        public AddressWrapper Address { get; private set; }

        public ChangeTrackingCollection<FriendEmailWrapper> Emails { get; private set; }

        private void InitializeCollectionProperties(Friend model)
        {
            if (model.Emails == null) throw new ArgumentException("Emails cannot be null");
            Emails = new ChangeTrackingCollection<FriendEmailWrapper>(
                model.Emails.Select(e => new FriendEmailWrapper(e)));
            RegisterCollection(Emails, model.Emails);
        }

        private void InitializeComplexProperties(Friend model)
        {
            if (model.Address == null) throw new ArgumentException("Address cannot be null");
            Address = new AddressWrapper(model.Address);
            RegisterComplex(Address);
        }
    }
}