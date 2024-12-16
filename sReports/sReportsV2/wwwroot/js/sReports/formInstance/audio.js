function audioFieldFunctions() {
    const audioContainers = document.querySelectorAll('.audio-container');

    audioContainers.forEach(container => {
        const startRecord = container.querySelector('.startRecord');
        const stopRecord = container.querySelector('.stopRecord');
        const pauseRecord = container.querySelector('.pauseRecord');
        const continueRecord = container.querySelector('.continueRecord');
        const timer = container.querySelector('.timer');
        const audioPlayback = container.querySelector('.audioPlayback');
        const audioInput = container.querySelector('input.audio-hid');
        let mediaRecorder;
        let audioChunks = [];
        let isRecording = false;
        let isPaused = false;
        let startTime;
        let elapsedTime = 0;
        let timerInterval;

        startRecord.addEventListener('click', function (event) {
            executeEventFunctions(event, true);
            startRecording();
        });

        stopRecord.addEventListener('click', function (event) {
            executeEventFunctions(event, true);
            stopRecording();
        });

        pauseRecord.addEventListener('click', function (event) {
            executeEventFunctions(event, true);
            pauseRecording();
        });

        continueRecord.addEventListener('click', function (event) {
            executeEventFunctions(event, true);
            continueRecording();
        });

        function startRecording() {
            startTime = Date.now() - elapsedTime;
            timerInterval = setInterval(updateTimer, 1000);
            timer.style.display = 'flex';
            isPaused = false;

            navigator.mediaDevices.getUserMedia({ audio: true })
                .then(stream => {
                    mediaRecorder = new MediaRecorder(stream);
                    mediaRecorder.start();
                    isRecording = true;
                    mediaRecorder.ondataavailable = function (event) {
                        audioChunks.push(event.data);
                    };
                    stopRecord.style.display = 'inline-block';
                    startRecord.style.display = 'none';
                    pauseRecord.style.display = 'inline-block';
                })
                .catch(error => {
                    console.error("Error accessing the microphone", error);
                });
        }

        function stopRecording() {
            if (isRecording) {
                clearInterval(timerInterval);
                mediaRecorder.stop();
                isRecording = false;
                timer.style.display = 'none';
                stopRecord.style.display = 'none';
                startRecord.style.display = 'inline-block';
                pauseRecord.style.display = 'none';
                continueRecord.style.display = 'none';
                mediaRecorder.onstop = function () {
                    const audioBlob = new Blob(audioChunks, { 'type': 'audio/mp3' });
                    audioChunks = [];
                    const reader = new FileReader();
                    reader.onload = function () {
                        const audioBase64 = reader.result.split(',')[1];

                        let filesData = [{
                            id: audioInput.id,
                            content: audioBase64
                        }];

                        let binaryFieldType = 'audio';
                        deleteExistingBinaryFromServer($(audioInput).val(), binaryFieldType);
                        sendAudioFileData(filesData, setResourceName, function (resourceName) {
                            let $fieldContainer = $(audioInput).closest(".repetitive-field");
                            toggleFileNameContainer($fieldContainer, binaryFieldType, resourceName);
                        });
                    };
                    reader.readAsDataURL(audioBlob);
                    const audioUrl = URL.createObjectURL(audioBlob);
                    audioPlayback.src = audioUrl;
                    stopRecord.disabled = true;
                };
            }
        }

        function pauseRecording() {
            if (isRecording && !isPaused) {
                clearInterval(timerInterval);
                isPaused = true;
                mediaRecorder.pause();
                pauseRecord.style.display = 'none';
                startRecord.style.display = 'none';
                stopRecord.style.display = 'inline-block';
                continueRecord.style.display = 'inline-block';
            }
        }

        function continueRecording() {
            if (isPaused) {
                startTime = Date.now() - elapsedTime * 1000;
                timerInterval = setInterval(updateTimer, 1000);
                isPaused = false;
                mediaRecorder.resume();
                continueRecord.style.display = 'none';
                pauseRecord.style.display = 'inline-block';
            }
        }

        function updateTimer() {
            const totalSeconds = 120 * 60; // 2 hours in seconds
            elapsedTime = Math.floor((Date.now() - startTime) / 1000);
            const minutes = Math.floor(elapsedTime / 60);
            const seconds = elapsedTime % 60;

            if (elapsedTime >= totalSeconds) {
                stopRecording(); // Stop recording when the limit is reached
                return;
            }

            timer.textContent = formatTime(minutes) + ':' + formatTime(seconds) + ' / 02:00';
        }

        function formatTime(time) {
            return time < 10 ? '0' + time : time;
        }
    });
}

async function sendAudioFileData(fileData, setFieldCallback, filesUploadedCallBack, url = '/Blob/CreateAudio') {
    var chunkSize = 1024 * 1024; // 1 MB chunk size

    for (let i = 0; i < fileData.length; i++) {
        var file = fileData[i];
        var content = file.content;

        var fileSize = getFileSize(content);

        if (fileSize === 0) {
            console.error('File size is undefined for file:', file);
            continue;
        }

        var start = 0;
        var end = Math.min(chunkSize, fileSize);
        var chunkIndex = 0;
        var totalChunks = Math.ceil(fileSize / chunkSize);

        while (start < fileSize) {
            var chunk = content.slice(start, end);
            var isLastChunk = chunkIndex === totalChunks - 1;
            var isLastElement = i === fileData.length - 1 && isLastChunk;

            await new Promise((resolve, reject) => {
                var fd = new FormData();
                fd.append('file', chunk);
                fd.append('isLastChunk', isLastChunk);

                callServer({
                    url: url,
                    data: fd,
                    processData: false,
                    contentType: false,
                    type: 'POST',
                    success: function (data) {
                        if (lastChunkUploaded(data)) {
                            if (setFieldCallback) {
                                console.log('setting field: ' + file.id);
                                setFieldCallback(file.id, data);
                            }
                            if (isLastElement) {
                                filesUploadedCallBack(data);
                            }
                        }
                        resolve();
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        handleResponseError(xhr);
                    }
                });
            });

            start = end;
            end = Math.min(start + chunkSize, fileSize);
            chunkIndex++;
        }
    }
}

function getFileSize(content) {
    if (content instanceof ArrayBuffer) {
        return content.byteLength;
    } else if (content instanceof Blob) {
        return content.size;
    } else if (typeof content === 'string') {
        return encodeURIComponent(content).replace(/%../g, 'x').length;
    } else {
        return 0;
    }
}

function lastChunkUploaded(data) {
    return data != "";
}