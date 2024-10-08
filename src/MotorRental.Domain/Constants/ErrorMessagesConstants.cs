﻿namespace MotorRental.Domain.Constants
{
    public static class ErrorMessagesConstants
    {
        public const string BADREQUEST_DEFAULT = "Invalid Parameters.";
        public const string SUCCESS_OK = "Operation succeeded.";

        public const string DELIVER_DRIVER_NOT_REGISTERED = "The driver is not registered.";
        public const string DELIVER_DRIVER_NOT_QUALIFIED = "Only delivery drivers qualified in category A can rent.";
        public const string DELIVER_DRIVER_RENT_ACTIVE = "You have a rent active.";
        public const string DELIVER_DRIVER_CNPJ_EXISTS = "CNPJ already registered for another user.";
        public const string DELIVER_DRIVER_LICENSE_NUMBER = "CNH already registered for another user.";
        public const string DELIVER_DRIVER_ILEGAL_AGE = "You are not of legal age.";

        public const string PHOTO_EMPTY = "Photo is empty.";
        public const string PHOTO_EXTENSION_INVALID = "Only .png and .bmg are accepted.";

        public const string START_DATE_REQUIRED_INVALID = "The motorcycle rental start date is required and must be from today.";

        public const string MOTORCYCLE_DOESNT_EXIST = "The reported motorcycle does not exists.";
        public const string NO_MOTORCYCLE_AVAILABLE = "There is no motorcycle available.";
        public const string MOTORCYCLE_RENT_ACTIVE = "The motorcycle have a rent active.";
        public const string MOTORCYCLE_LICENSE_PLATE_EXISTS = "The vehicle license plate already exists.";

        public const string PLAN_NOT_REGISTERED = "The selected plan is not registered.";

        public const string RENTAL_DOESNT_EXIST = "The reported rent does not exists.";

        public const string ROLE_DOESNT_EXIST = "The role reported does not exist.";
    }
}
