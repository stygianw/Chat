using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MvcApplication14.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage="Логин не должен быть пустым!")]
        [Display(Name="Введите ваш логин: ")]
        public string Login { get; set; }

        [Required(ErrorMessage="Пароль не должен быть пустым!")]
        [Display(Name="Введите ваш пароль: ")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name="Подтвердите пароль: ")]
        [Compare("Password", ErrorMessage="Пароли не совпадают")]
        [DataType(DataType.Password)]
        public string RetypePassword { get; set; }
    }
}