var fs = require("fs"),
    path = require("path"),
    tape = require("tape"),
    queue = require("d3-queue"),
    d3 = require("../");

tape("formatLocale({decimal: decimal}) observes the specified decimal point", function(test) {
  test.equal(d3.formatLocale({decimal: "|"}).format("06.2f")(2), "002|00");
  test.equal(d3.formatLocale({decimal: "/"}).format("06.2f")(2), "002/00");
  test.end();
});

tape("formatLocale({currency: [prefix, suffix]}) observes the specified currency prefix and suffix", function(test) {
  test.equal(d3.formatLocale({decimal: ".", currency: ["฿", ""]}).format("$06.2f")(2), "฿02.00");
  test.equal(d3.formatLocale({decimal: ".", currency: ["", "฿"]}).format("$06.2f")(2), "02.00฿");
  test.end();
});

tape("formatLocale({grouping: null}) does not perform any grouping", function(test) {
  test.equal(d3.formatLocale({decimal: ".", grouping: null}).format("012,.2f")(2), "000000002.00");
  test.end();
});

tape("formatLocale({grouping: [sizes…]}) observes the specified group sizes", function(test) {
  test.equal(d3.formatLocale({decimal: ".", grouping: [3], thousands: ","}).format("012,.2f")(2), "0,000,002.00");
  test.equal(d3.formatLocale({decimal: ".", grouping: [2], thousands: ","}).format("012,.2f")(2), "0,00,00,02.00");
  test.equal(d3.formatLocale({decimal: ".", grouping: [2, 3], thousands: ","}).format("012,.2f")(2), "00,000,02.00");
  test.equal(d3.formatLocale({decimal: ".", grouping: [3, 2, 2, 2, 2, 2, 2], thousands: ","}).format(",d")(1e12), "10,00,00,00,00,000");
  test.end();
});

tape("formatLocale(…) can format numbers using the Indian numbering system.", function(test) {
  var format = d3.formatLocale(require("../locale/en-IN")).format(",");
  test.equal(format(10), "10");
  test.equal(format(100), "100");
  test.equal(format(1000), "1,000");
  test.equal(format(10000), "10,000");
  test.equal(format(100000), "1,00,000");
  test.equal(format(1000000), "10,00,000");
  test.equal(format(10000000), "1,00,00,000");
  test.equal(format(10000000.4543), "1,00,00,000.4543");
  test.equal(format(1000.321), "1,000.321");
  test.equal(format(10.5), "10.5");
  test.equal(format(-10), "-10");
  test.equal(format(-100), "-100");
  test.equal(format(-1000), "-1,000");
  test.equal(format(-10000), "-10,000");
  test.equal(format(-100000), "-1,00,000");
  test.equal(format(-1000000), "-10,00,000");
  test.equal(format(-10000000), "-1,00,00,000");
  test.equal(format(-10000000.4543), "-1,00,00,000.4543");
  test.equal(format(-1000.321), "-1,000.321");
  test.equal(format(-10.5), "-10.5");
  test.end();
});

tape("formatLocale({thousands: separator}) observes the specified group separator", function(test) {
  test.equal(d3.formatLocale({decimal: ".", grouping: [3], thousands: " "}).format("012,.2f")(2), "0 000 002.00");
  test.equal(d3.formatLocale({decimal: ".", grouping: [3], thousands: "/"}).format("012,.2f")(2), "0/000/002.00");
  test.end();
});

tape("locale data is valid", function(test) {
  fs.readdir("locale", function(error, locales) {
    if (error) throw error;
    var q = queue.queue(1);
    locales.forEach(function(locale) {
      if (!/\.json$/i.test(locale)) return;
      q.defer(testLocale, path.join("locale", locale));
    });
    q.awaitAll(function(error) {
      if (error) throw error;
      test.end();
    });
  });

  function testLocale(locale, callback) {
    fs.readFile(locale, "utf8", function(error, locale) {
      if (error) return void callback(error);
      locale = JSON.parse(locale);
      test.equal("currency" in locale, true);
      test.equal("decimal" in locale, true);
      test.equal("grouping" in locale, true);
      test.equal("thousands" in locale, true);
      locale = d3.formatLocale(locale);
      callback(null);
    });
  }
});
