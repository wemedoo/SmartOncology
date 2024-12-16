db.form.find({}).forEach(function (form) {
	form.Chapters.forEach(function (chapter) {
		chapter.Pages.forEach(function (page) {
			page.ListOfFieldSets.forEach(function (list) {
				list.forEach(function (fieldSet) {
					fieldSet.Fields.forEach(function (field) {
						delete field.InstanceId;
						delete field.Value;
						delete field.ValueLabel;
					});
					delete fieldSet.InstanceId;
				});
			});
		});
	});

	db.form.replaceOne({ _id: form._id }, form);
});

db.forminstance.find({}).forEach(function (forminstance) {
	delete forminstance.Fields;

	db.forminstance.replaceOne({ _id: forminstance._id }, forminstance);
});