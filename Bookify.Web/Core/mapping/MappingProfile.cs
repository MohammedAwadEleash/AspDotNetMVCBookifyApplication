namespace Bookify.Web.Core.mapping
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {

            //Categories
            CreateMap<Category, CategoryViewModel>();
            CreateMap<CategoryFormViewModel, Category>().ReverseMap();

            // Here  The  Categories that came from database  are converted To SelectListItem Type  to  be displayed in view as a  dropdownlist 
            CreateMap<Category, SelectListItem>()
                .ForMember(des => des.Value, opt => opt.MapFrom(src => src.Id))
                .ForMember(des => des.Text, opt => opt.MapFrom(src => src.Name));



            //Authors
            CreateMap<Author, AuthorViewModel>();
            CreateMap<AuthorFormViewModel, Author>().ReverseMap();

            CreateMap<Author, SelectListItem>()
               .ForMember(des => des.Value, opt => opt.MapFrom(src => src.Id))
               .ForMember(des => des.Text, opt => opt.MapFrom(src => src.Name));




            //Books
            CreateMap<BookFormViewModel, Book>()
                 .ReverseMap()
                .ForMember(des => des.Categories, op => op.Ignore());

            CreateMap<Book, BookViewModel>()
            .ForMember(des => des.Author, opt => opt.MapFrom(src => src.Author!.Name))
            .ForMember(des => des.Categories, opt => opt.MapFrom(src => src.Categories.Select(bc => bc.Category!.Name)));

            // this is  specific to Book Index Page 
            CreateMap<Book, BookRowViewModel>()
          .ForMember(des => des.Author, opt => opt.MapFrom(src => src.Author!.Name))
          .ForMember(des => des.Categories, opt => opt.MapFrom(src => src.Categories.Select(bc => bc.Category!.Name)));
            //BookSearchResultViewModel:
            CreateMap<Book, BookSearchResultViewModel>()
          .ForMember(des => des.Author, opt => opt.MapFrom(src => src.Author!.Name));
            //BookDto:
            CreateMap<BookDto, BookViewModel>();



            // BookCopy:
            CreateMap<BookCopy, BookCopyViewModel>()
                .ForMember(des => des.BookTitle, opt => opt.MapFrom(src => src.Book!.Title))
                //.ForMember(des => des.BookId, opt => opt.MapFrom(src => src.Book!.Id))
                .ForMember(des => des.BookThumbnailUrl, opt => opt.MapFrom(sr => sr.Book!.ImageThumbnailUrl));

            CreateMap<BookCopyFormViewModel, BookCopy>().ReverseMap();

            //User:
            CreateMap<ApplicationUser, UserViewModel>();
            CreateMap<UserFormViewModel, ApplicationUser>().ForMember(des => des.NormalizedUserName, op => op.MapFrom(src => src.UserName.ToUpper()))
          .ForMember(des => des.NormalizedEmail, op => op.MapFrom(src => src.Email.ToUpper())).ReverseMap();
            // CreateUserDto:
            CreateMap<UserFormViewModel, CreateUserDto>();

            // Governorate,Area:

            CreateMap<Area, SelectListItem>()
                 .ForMember(des => des.Text, opt => opt.MapFrom(scr => scr.Name))
                 .ForMember(des => des.Value, opt => opt.MapFrom(scr => scr.Id));

            CreateMap<Governorate, SelectListItem>()
                .ForMember(des => des.Text, opt => opt.MapFrom(src => src.Name))
                .ForMember(des => des.Value, opt => opt.MapFrom(src => src.Id));

            //  SubscriberSearchResultViewModel , Subscriber , SubscriberViewModel 
            CreateMap<SubscriberFormViewModel, Subscriber>().ReverseMap();
            CreateMap<Subscriber, SubscriberSearchResultViewModel>()
                .ForMember(des => des.FullName, op => op.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            CreateMap<Subscriber, SubscriberViewModel>()
            .ForMember(des => des.FullName, op => op.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area!.Name))
            .ForMember(dest => dest.Governorate, opt => opt.MapFrom(src => src.Governorate!.Name));

            // Subscription 

            CreateMap<Subscription, SubscriptionViewModel>();



            // Rentals , RentalCopies

            CreateMap<Rental, RentalViewModel>();
            CreateMap<RentalCopy, RentalCopyViewModel>();
            CreateMap<ReturnCopyViewModel, ReturnCopyDto>();


            // RentalCopy  ,CopyHistoryViewModel

            CreateMap<RentalCopy, CopyHistoryViewModel>()
                .ForMember(dest => dest.RentalNumber, opt => opt.MapFrom(src => src.Rental!.Id))
                .ForMember(dest => dest.SubscriberName, opt => opt.MapFrom(src => $"{src.Rental!.subscriber!.FirstName} {src.Rental!.subscriber!.LastName}"))
                .ForMember(dest => dest.SubscriberMobile, opt => opt.MapFrom(src => src.Rental!.subscriber!.MobileNumber))
                .ForMember(dest => dest.SubscriberEmail, opt => opt.MapFrom(src => src.Rental!.subscriber!.Email));


            //ChartItemViewModel
            CreateMap<KeyValuePairDto, ChartItemViewModel>()
            .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.Key));
        }

    }
}
