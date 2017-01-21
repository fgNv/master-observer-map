var vendorsJsPaths = ["node_modules/jquery/dist/jquery.js",
               "node_modules/signalr/jquery.signalR.js"];

var gulp = require('gulp');
var concat = require('gulp-concat');

gulp.task('default', function () {
    return gulp.src(vendorsJsPaths)
               .pipe(concat('vendors.js'))
               .pipe(gulp.dest('./dist/'));
});