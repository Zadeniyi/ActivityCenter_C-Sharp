using System;
using System.ComponentModel.DataAnnotations;
using ActivityCenter.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;


namespace ActivityCenter.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}
// ---------------------------------------------------------------------------------------------------------------------3

        [Required]
        [MinLength(2, ErrorMessage = " Your First Name muct contain at least 2 characters.")]
        public string FirstName {get;set;}
// --------------------------------------------------------------------------------------------------------------------

        [Required]
        [MinLength(2, ErrorMessage = " Your Last Name muct contain at least 2 characters.")]
        public string LastName {get;set;}
// --------------------------------------------------------------------------------------------------------------------

        [Required(ErrorMessage = "Please enter a valid email address")]
        [EmailAddress]
        public string Email {get;set;}
// --------------------------------------------------------------------------------------------------------------------

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage ="Your Password is too short")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[$@$!%*#?&])[A-Za-z\d$@$!%*#?&]{8,}$", ErrorMessage = "Your password must have at least (1) letter, (1) number and (1) special character and also be (8) characters long")]
        public string Password {get; set;}
// --------------------------------------------------------------------------------------------------------------------

        public DateTime Created_at { get; set; } = DateTime.Now;
// --------------------------------------------------------------------------------------------------------------------

        public DateTime Updated_at { get; set; } = DateTime.Now;
// --------------------------------------------------------------------------------------------------------------------
        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConformPassword {get;set;}
// ---------------------------------------------------------------------------------------------------------------------3
        public List<ActConnect> UsertoAct {get;set;}
    }

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public class LoginUser{
        
        [Required(ErrorMessage = "Please provide your correct email address!")]
        [EmailAddress]
        public string LoginEmail {get;set;}
// ---------------------------------------------------------------------------------------------------------------------3

        
        [Required(ErrorMessage = "Please provide your correct email address!")]
        [EmailAddress]
        public string LoginPassword {get;set;}
    }
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public class NewActivity{
        [Key]
        public int ActivityId {get;set;}
// ---------------------------------------------------------------------------------------------------------------------
        [Required]
        [MinLength(2, ErrorMessage ="Your Activity Title is too short. Ya need 2 characters!!")]
        public string Title {get;set;}
// ---------------------------------------------------------------------------------------------------------------------
        [Required]
        [RegularExpression(@"\b((1[0-2]|0?[1-9]):([0-5][0-9]) ([AaPp][Mm]))", ErrorMessage="Please make sure your time is chosen HH/MM AM or PM")]
        public string Time {get; set;}
// ---------------------------------------------------------------------------------------------------------------------
        [Required]
        public DateTime ActivityDate {get;set;}
// ---------------------------------------------------------------------------------------------------------------------
        [Required]
        [RegularExpression(@"^[+]?\d+([.]\d+)?$", ErrorMessage = "That duration is not possible, fool.")]
        public int Duration {get;set;}
// ---------------------------------------------------------------------------------------------------------------------
        [MinLength(10, ErrorMessage ="Please enter a description with at least 10 characters.")]
        public string Description {get;set;}
// ---------------------------------------------------------------------------------------------------------------------
        public User Coordinator {get;set;}
// ---------------------------------------------------------------------------------------------------------------------
        public int UserId {get;set;}
// ---------------------------------------------------------------------------------------------------------------------
        public DateTime Created_at {get;set;} = DateTime.Now;
// ---------------------------------------------------------------------------------------------------------------------
        public DateTime Updated_at {get;set;} = DateTime.Now;
// ---------------------------------------------------------------------------------------------------------------------
        public List<ActConnect> ActtoUser {get;set;}

    }
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public class ActConnect
    {
        [Key]
        public int ActConnectId {get;set;}
// ---------------------------------------------------------------------------------------------------------------------
        public int UserId {get;set;}
// ---------------------------------------------------------------------------------------------------------------------
        public int ActivityId {get;set;}
// ---------------------------------------------------------------------------------------------------------------------
        public User User {get;set;}
// ---------------------------------------------------------------------------------------------------------------------
        public NewActivity Activity {get;set;}
    }
}