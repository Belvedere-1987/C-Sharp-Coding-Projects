function ProduceGrabber(item) {
    var self = this;
    self.grocery = ko.observable(item);
   }

function ProduceViewModel() {
    var self = this;

    self.availableProduce = [
        { prod: "Nothing Selected", price: 0},
        { prod: "One full Watermelon", price: 5 },
        { prod: "One Pound of Apples", price: 3 },
        { prod: "One Pound of Oranges", price: 4 },
        { prod: "One Bunch of Bananas", price: 6 },
        { prod: "One Pound of Strawberries", price: 2 },
        { prod: "One Pound of Broccoli", price: 3 },
        { prod: "One Pound of Tomatoes", price: 2 },
        { prod: "One Pound of Green Beans", price: 4 },
        { prod: "One Pound of Asparagus", price: 6 },
        { prod: "One Pound of Carrots", price: 4 }
    ];

    self.produce = ko.observableArray([
        new ProduceGrabber(self.availableProduce[0])
    ]);
    

    self.prodTotal = ko.computed(function () {
        var total = 0;
        for (var i = 0; i < self.produce().length; i++)
            total += self.produce()[i].grocery().price;
        return total;
    });

    self.addVegetable = function () {
        self.produce.push(new ProduceGrabber("", self.availableProduce[0]));
    }
    self.removeVegetable = function(picked) { self.produce.remove(picked) }

}

ko.applyBindings(new ProduceViewModel());

