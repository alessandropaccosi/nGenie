(function ($) {
    $.fn.responsiveCanvas = function (method) {
        var methods = {
            // init functions
            init: function (options) {
                return this.each(function (i) {
                    var c = this, // THE canvas
                        ct = c.getContext('2d'), // check the context
                        $container = $(c).parent(); // parent container object

                    // Run function when browser loads & resizes
                    $(window).bind("load resize", function () {
                        methods.respondCanvas.call(c, $container);
                    });
                });

            },

            // the responsive function !
            respondCanvas: function ($container) {
                $(this).css({
                    'width': $container.innerWidth(), // max width
                    'height': $container.innerHeight() // max height
                });
            }
        }

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method "' + method + '" does not exist in pluginName plugin!');
        }
    }
})(jQuery);