$(document).ready(function(){



    var books = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        remote: {
            url: '/Search/Find?query=%QUERY',
            wildcard: '%QUERY'
        }
    });


    $('#Search').typeahead(
        {

            minLength: 4,
            highlight: true


        },
        {
            name: 'book',
            limit: 100,
            display: 'title',
            source: books,
            templates: {
                header: '<h2 class="p-3 fw-bold color-bounce bg-secondary">Books 📘 🔎:</h2>',
                empty: [
                    `'<div class="  p-2">   <h3 class=" selectable-text m-3 fw-bold text-danger">No book Was found ⁉ </h3>
                   <span class="underline-animate f-xs m-2 text-gray text-bold">
                   Please enter (the correct name) of the book 📘 or author 🖋️ </span>
                   </div>`

                ].join('\n'),
                suggestion: Handlebars.compile('<div class="py-2"><span>{{title}}</span><br/><span class="f-xs text-gray-400">by {{author}}</span></div>')
            }
        }).on('typeahead:select', function (e, book) {
            window.location.replace(`/Search/BookDetails?bookkey=${book.key}`);
        });
}); 