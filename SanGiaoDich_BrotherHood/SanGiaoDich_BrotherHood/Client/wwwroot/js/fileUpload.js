window.selectFile = async function () {
    return new Promise((resolve, reject) => {
        const input = document.createElement('input');
        input.type = 'file';
        input.accept = 'image/*';
        input.multiple = true; // Cho phép chọn nhiều file
        input.onchange = () => {
            const files = input.files;
            if (files.length > 0) {
                const promises = [];
                for (let i = 0; i < files.length; i++) {
                    promises.push(new Promise((res, rej) => {
                        const reader = new FileReader();
                        reader.onload = (e) => {
                            res({ dataUrl: e.target.result });
                        };
                        reader.onerror = (error) => rej(error);
                        reader.readAsDataURL(files[i]);
                    }));
                }
                Promise.all(promises)
                    .then(results => resolve(results)) // Trả về mảng các hình ảnh đã chọn
                    .catch(err => reject(err));
            } else {
                reject('No files selected');
            }
        };
        input.click();
    });
};
