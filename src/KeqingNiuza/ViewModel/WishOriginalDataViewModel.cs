using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using KeqingNiuza.Common;
using KeqingNiuza.Model;
using KeqingNiuza.Wish;

namespace KeqingNiuza.ViewModel
{
    public class WishOriginalDataViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public List<WishData> WishDataList { get; set; }


        private List<WishData> _FilteredWishData;
        public List<WishData> FilteredWishData
        {
            get { return _FilteredWishData; }
            set
            {
                _FilteredWishData = value;
                OnPropertyChanged();
            }
        }


        public List<string> WishTypeList { get; set; }

        private string _SelectedWishType = "---";

        public string SelectedWishType
        {
            get { return _SelectedWishType; }
            set
            {
                _SelectedWishType = value;
                OnPropertyChanged();
                FilterWishData();
            }
        }

        public List<string> ItemRankList { get; set; }

        private string _SelectedItemRank = "-";
        public string SelectedItemRank
        {
            get { return _SelectedItemRank; }
            set
            {
                _SelectedItemRank = value;
                OnPropertyChanged();
                FilterWishData();
            }
        }

        public List<WishEvent> WishEventList { get; set; }

        private WishEvent _SelectedWishEvent;
        public WishEvent SelectedWishEvent
        {
            get { return _SelectedWishEvent; }
            set
            {
                _SelectedWishEvent = value;
                OnPropertyChanged();
                SelectedWishType = "---";
                FilterWishData();
            }
        }


        private bool _ToggleButton_Search_IsChecked;
        public bool ToggleButton_Search_IsChecked
        {
            get { return _ToggleButton_Search_IsChecked; }
            set
            {
                _ToggleButton_Search_IsChecked = value;
                OnPropertyChanged();
                FilterWishData();
            }
        }

        private string _TextBox_Search_Text = "";
        public string TextBox_Search_Text
        {
            get { return _TextBox_Search_Text; }
            set
            {
                _TextBox_Search_Text = value;
                OnPropertyChanged();
                FilterWishData();
            }
        }



        public WishOriginalDataViewModel(UserData userData)
        {
            WishTypeList = new List<string> { "---", "新手祈愿", "常驻祈愿", "角色活动祈愿", "武器活动祈愿" };
            ItemRankList = new List<string> { "-", "3", "4", "5" };
            var json = File.ReadAllText("Resource\\List\\WishEventList.json");
            var eventlist = JsonSerializer.Deserialize<List<WishEvent>>(json, Const.JsonOptions);
            WishEventList = eventlist.Prepend(Const.ZeroWishEvent).ToList();
            SelectedWishEvent = WishEventList[0];
            json = File.ReadAllText(userData.WishLogFile);
            var datas = JsonSerializer.Deserialize<List<WishData>>(json, Const.JsonOptions);
            WishDataList = ComputeGuarantee(datas);
            FilteredWishData = WishDataList;
        }

        private List<WishData> ComputeGuarantee(List<WishData> datas)
        {
            var groups = datas.GroupBy(x => x.WishType);
            List<WishData> result = new List<WishData>(datas.Count);
            foreach (var group in groups)
            {
                var list = group.OrderBy(x => x.Id).ToList();
                int tmp = -1;
                for (int i = 0; i < group.Count(); i++)
                {
                    list[i].Guarantee = i - tmp;
                    if (list[i].RankType == 5)
                    {
                        tmp = i;
                    }
                }
                result.AddRange(list);
            }
            return result.OrderByDescending(x => x.Id).ToList();
        }


        public void ResetFilter()
        {
            SelectedWishType = "---";
            SelectedItemRank = "-";
            SelectedWishEvent = WishEventList[0];
            ToggleButton_Search_IsChecked = false;
            TextBox_Search_Text = "";
            FilteredWishData = WishDataList;
        }

        public void FilterWishData(bool delay = false)
        {
            Task.Run(() =>
             {
                 if (delay)
                 {
                     //防止搜索框升起时，完成数据过滤卡住UI线程
                     //现在搜索框没动画了
                     //Thread.Sleep(160);
                 }
                 var tmp = WishDataList;
                 switch (SelectedWishType)
                 {
                     case "新手祈愿":
                         tmp = tmp.FindAll(x => x.WishType == WishType.Novice);
                         break;
                     case "常驻祈愿":
                         tmp = tmp.FindAll(x => x.WishType == WishType.Permanent);
                         break;
                     case "角色活动祈愿":
                         tmp = tmp.FindAll(x => x.WishType == WishType.CharacterEvent);
                         break;
                     case "武器活动祈愿":
                         tmp = tmp.FindAll(x => x.WishType == WishType.WeaponEvent);
                         break;
                 }
                 if (SelectedItemRank != "-")
                 {
                     tmp = tmp.FindAll(x => x.RankType == int.Parse(SelectedItemRank));
                 }
                 if (SelectedWishEvent.Name != "---")
                 {
                     tmp = tmp.FindAll(x => x.WishType == SelectedWishEvent.WishType && x.Time > SelectedWishEvent.StartTime && x.Time < SelectedWishEvent.EndTime);
                 }
                 if (ToggleButton_Search_IsChecked && TextBox_Search_Text != "")
                 {
                     tmp = tmp.FindAll(x => x.Name.Contains(TextBox_Search_Text));
                 }
                 FilteredWishData = tmp;
             });
        }

    }
}
