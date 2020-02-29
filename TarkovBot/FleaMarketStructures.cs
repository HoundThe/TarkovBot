using System;
using System.Collections.Generic;
using System.Text;

namespace TarkovBot
{
    public class SystemData
    {
        public string buyerNickname { get; set; }
        public string soldItem { get; set; }
        public int itemCount { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public string location { get; set; }
    }

    public class Items
    {
        public string stash { get; set; }
        public List<Item> data { get; set; }
    }

    public class Message
    {
        public string _id { get; set; }
        public string uid { get; set; }
        public int type { get; set; }
        public double dt { get; set; }
        public string templateId { get; set; }
        public SystemData systemData { get; set; }
        public Items items { get; set; }
        public int maxStorageTime { get; set; }
        public bool hasRewards { get; set; }
    }

    public class AttachmentsData
    {
        public List<Message> messages { get; set; }
        public List<object> profiles { get; set; }
    }

    public class AttachmentsResponse
    {
        public int err { get; set; }
        public object errmsg { get; set; }
        public AttachmentsData data { get; set; }
    }


    public class MessageResponseData
    {
        public int type { get; set; }
        public Message message { get; set; }
        public int attachmentsNew { get; set; }
        public int @new { get; set; }
        public bool pinned { get; set; }
        public string _id { get; set; }
    }

    public class MessageResponse
    {
        public int err { get; set; }
        public object errmsg { get; set; }
        public List<MessageResponseData> data { get; set; }
    }

    public enum SortBy
    {
        ID = 0,
        BarteringOffers = 2,
        MerchantRating = 3,
        Price = 5,
        Expiry = 6
    }

    public enum SortDirection
    {
        Ascending = 0,

        Descending = 1
    }

    public enum Currency
    {
        Any = 0,
        Rouble = 1,
        Dollar = 2,
        Euro = 3
    }

    public enum Owner
    {
        Any = 0,
        Traders = 1,
        Player = 2
    }

    public class FleaRequest
    {
        public ulong page { get; set; }
        public ulong limit { get; set; }
        public SortBy sortType { get; set; }
        public SortDirection sortDirection { get; set; }
        public Currency currency { get; set; }
        public ulong priceFrom { get; set; }
        public ulong priceTo { get; set; }
        public ulong quantityFrom { get; set; }
        public ulong quantityTo { get; set; }
        public ulong conditionFrom { get; set; }
        public ulong conditionTo { get; set; }
        public bool oneHourExpiration { get; set; }
        public bool removeBartering { get; set; }
        public Owner offerOwnerType { get; set; }
        public bool onlyFunctional { get; set; }
        public bool updateOfferCount { get; set; }
        public string handbookId { get; set; }
        public string linkedSearchId { get; set; }
        public string neededSearchId { get; set; }
        public ulong tm { get; set; }
    }

    public class Price
    {
        public string SchemaId { get; set; }
        public ulong Min { get; set; }
        public ulong Max { get; set; }
        public ulong Avg { get; set; }
    }

    public class GetPriceRequest
    {
        public string templateId { get; set; }
    }

    public class SearchResponse
    {
        public ErrorResponse error { get; set; }
        public SearchResult? data { get; set; }
    }
    public class SearchResult
    {
        public Dictionary<string, ulong> categories { get; set; }
        public List<Offer> offers { get; set; }
        public ulong offerCount { get; set; }
        public string selectedCategory { get; set; }
    }
    public class Offer
    {
        public string _id { get; set; }
        public string intId { get; set; }
        public User user { get; set; }
        public List<Item> items { get; set; }
        public ulong itemCost { get; set; }

        public List<Requirement> requirements { get; set; }
        public ulong requirementCost { get; set; }
        public ulong summaryCost { get; set; }
        public bool sellInOnePiece { get; set; }
        public long startTime { get; set; }
        public long endTime { get; set; }
        public ulong loyaltyLevel { get; set; }
    }
    public class User
    {
        public string id { get; set; }
        public ulong memberType { get; set; }
        public string? nickname { get; set; }
        public double? rating { get; set; }
        public bool? isRatingGrowing { get; set; }
        public string? avatar { get; set; }
    }

    public class Requirement
    {
        public string _tpl { get; set; }
        public ulong count { get; set; }
    }
    public class ErrorResponse
    {
        public ulong err { get; set; }
        public string errmsg { get; set; }
    }

    public class BuyItemResponse
    {
        public ulong err { get; set; }
        public string errmsg { get; set; }
        public ResponseData data { get; set; }
    }

    public class ResponseData
    {
        public List<BadRequest> badRequest { get; set; }
        public List<object> quests { get; set; }
        public List<object> ragFairOffers { get; set; }
        public List<object> builds { get; set; }
    }

    public class BadRequest
    {
        public int index { get; set; }
        public string errmsg { get; set; }
        public int err { get; set; }
    }
    public class BuyItemRequest
    {
        public string Action { get; set; }
        public List<BuyOffer> offers { get; set; }
    }
    public class BuyItem
    {
        public string item { get; set; }
        public double count { get; set; }
    }
    public class BuyOffer
    {
        public string id { get; set; }
        public ulong count { get; set; }
        public List<BarterItem> items { get; set; }
    }

    public class BarterItem
    {
        public string id { get; set; }
        public double count { get; set; }
    }
    public class MoveFleaItemRequest
    {
        public List<BuyItemRequest> data { get; set; }
        public long tm { get; set; }
    }

    public class SellFleaItemRequest
    {
        public List<SellItemRequest> data { get; set; }
        public long tm { get; set; }
    }

    public class SellItemRequest
    {
        public string Action { get; set; }
        public bool sellInOnePiece { get; set; }
        public List<string> items { get; set; }
        public List<SellRequirement> requirements { get; set; }
    }
    public class SellRequirement
    {
        public string _tpl { get; set; }
        public ulong count { get; set; }
        public ulong level { get; set; }
        public byte side { get; set; }
        public bool onlyFunctional { get; set; }
    }

    public class Data
    {
        public Object items { get; set; }
        public List<string> badRequest { get; set; }
        public List<object> quests { get; set; }
        public List<Offer> ragFairOffers { get; set; }
        public List<object> builds { get; set; }
    }

    public class Response
    {
        public int err { get; set; }
        public string errmsg { get; set; }
        public Object data { get; set; }
    }

    public enum BuyStatus
    {
        Success,
        OfferNotFound,
        InventoryFull,
        ProfileLocked,
        NotEnoughMoney,
        OtherError,
        BackendError
    }

    public enum SellStatus
    {
        Success,
        NoMoneyForTax,
        NoAvailableOffer,
        OtherErr,
        BackendError
    }

}
