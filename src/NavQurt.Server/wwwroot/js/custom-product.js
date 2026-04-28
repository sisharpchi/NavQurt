$('#treeBasic')
    // listen for event
    .on('changed.jstree', function (e, data) {
        let categoriesIds = [];
        let productElements = document.querySelectorAll('.product');
        for (let categoryElem of data.selected) {
            categoriesIds.push(data.instance.get_node(categoryElem)['li_attr']['data-cat-id']);
        }
        for (let productElement of productElements) {
            console.log(categoriesIds.includes(productElement.getAttribute('data-category-id')), productElement.getAttribute('data-category-id'), categoriesIds);
            if (categoriesIds.includes(productElement.getAttribute('data-category-id')) || categoriesIds.includes('all')) {
                productElement.className = 'product'
            } else productElement.className = 'product d-none'
        }
    }).jstree(
        {
            'core': {
                'themes': {
                    'responsive': false
                },
                "check_callback": true
            },
            'types': {
                'default': {
                    'icon': 'icofont icofont-folder font-theme'
                },
                'file': {
                    'icon': 'icofont icofont-file-alt font-theme'
                },
                'plus': {
                    'icon': 'fa fa-plus-circle font-theme'
                }
            },
            'plugins': ['types', 'contextmenu']
        }
    );