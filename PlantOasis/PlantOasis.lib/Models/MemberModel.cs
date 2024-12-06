using dufeksoft.lib.Model;
using dufeksoft.lib.UI;
using PlantOasis.lib.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantOasis.lib.Models
{
    public class MemberModel : _BaseModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id musí byť zadané")]
        public int MemberId { get; set; }
        [Email(ErrorMessage = "Neplatný tvar e-mailovej adresy")]
        [Display(Name = "Užívateľské meno (e-mail)")]
        [Required(ErrorMessage = "Užívateľské meno (e-mail) musí byť zadané")]
        public string Name { get; set; }

        [Display(Name = "Povolený")]
        public bool IsApproved { get; set; }
        [Display(Name = "Uzamknutý")]
        public bool IsLockedOut { get; set; }


        [Display(Name = "Administrátor")]
        public bool IsAdminUser { get; set; }
        [Display(Name = "Zákazník")]
        public bool IsCustomerUser { get; set; }

        [Display(Name = "Heslo")]
        public string Password { get; set; }
        [Display(Name = "Zopakované heslo")]
        public string PasswordRepeat { get; set; }

        public string IsCustomerEdit { get; set; }

        public override bool IsNew
        {
            get
            {
                return this.MemberId <= 0;
            }
        }

        public void CopyDataFrom(Member src)
        {
            this.MemberId = src.MemberId;
            this.Name = src.Name;
            this.IsApproved = src.IsApproved;
            this.IsLockedOut = src.IsLockedOut;
            this.IsAdminUser = src.IsAdminUser;
            this.IsCustomerUser = src.IsCustomerUser;
            this.Password = src.Password;
            this.PasswordRepeat = src.PasswordRepeat;
        }

        public void CopyDataTo(Member trg)
        {
            trg.MemberId = this.MemberId;
            trg.Name = this.Name;
            trg.Username = this.Name;
            trg.Email = this.Name;
            trg.IsApproved = this.IsApproved;
            trg.IsLockedOut = this.IsLockedOut;
            trg.IsAdminUser = this.IsAdminUser;
            trg.IsCustomerUser = this.IsCustomerUser;
            trg.Password = this.Password;
            trg.PasswordRepeat = this.PasswordRepeat;
        }

        public static MemberModel CreateCopyFrom(Member src)
        {
            MemberModel trg = new MemberModel();
            trg.CopyDataFrom(src);

            return trg;
        }

        public static Member CreateCopyFrom(MemberModel src)
        {
            Member trg = new Member();
            src.CopyDataTo(trg);

            return trg;
        }

        public static Member CreateCopyFrom(RegisterModel src)
        {
            Member trg = new Member();
            trg.MemberId = 0;
            trg.Name = src.Email;
            trg.Username = src.Email;
            trg.Email = src.Email;
            trg.IsApproved = true;
            trg.IsLockedOut = false;
            trg.IsAdminUser = false;
            trg.IsCustomerUser = true;

            trg.Password = src.RegisterPassword;
            trg.PasswordRepeat = src.RepeatRegisterPassword;

            return trg;
        }
    }
    public class LostPasswordModel : _BaseModel
    {
        [Display(Name = "Váš e-mail")]
        [Email(ErrorMessage = "Neplatný tvar e-mailovej adresy")]
        [Required(ErrorMessage = "Váš e-mail musí byť zadaný")]
        public string Email { get; set; }
    }

    public class ChangePasswordModel : _BaseModel
    {
        [Required(ErrorMessage = "Staré heslo musí byť zadané")]
        [Display(Name = "Staré heslo")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Nové heslo musí byť zadané")]
        [Display(Name = "Nové heslo")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Zopakované nové heslo musí byť zadané")]
        [Display(Name = "Zopakované nové heslo")]
        public string NewPasswordRepeat { get; set; }
    }

    public class MemberListModel : _PagingModel
    {
        public List<MemberModel> Items { get; set; }

        public static MemberListModel CreateCopyFrom(List<Member> srcArray)
        {
            MemberListModel trgArray = new MemberListModel();
            trgArray.ItemsPerPage = (int)srcArray.Count;
            trgArray.TotalItems = (int)srcArray.Count;
            trgArray.Items = new List<MemberModel>(srcArray.Count + 1);

            foreach (Member src in srcArray)
            {
                trgArray.Items.Add(MemberModel.CreateCopyFrom(src));
            }

            return trgArray;
        }
    }

    public class MemberDropDown : CmpDropDown
    {
        public MemberDropDown()
        {
        }

        public static MemberDropDown CreateCustomerUserListFromRepository(bool allowNull, string emptyText = "[ žiadny užívateľ ]", bool allowAll = false, string allText = "[ všetci užívatelia ]")
        {
            MemberRepository memberRepository = new MemberRepository();
            return MemberDropDown.CreateCopyFrom(memberRepository.GetCustomerUsers(), allowNull, emptyText, allowAll, allText);
        }

        public static MemberDropDown CreateCopyFrom(List<Member> memberList, bool allowNull, string emptyText, bool allowAll, string allText)
        {
            MemberDropDown ret = new MemberDropDown();

            if (allowNull)
            {
                ret.AddItem(emptyText, "0", null);
            }
            if (allowAll)
            {
                ret.AddItem(allText, "-1", null);
            }
            foreach (Member memberItem in memberList)
            {
                MemberModel memberModel = MemberModel.CreateCopyFrom(memberItem);
                ret.AddItem(memberItem.Name, memberItem.MemberId.ToString(), memberModel);
            }

            return ret;
        }
    }
}
