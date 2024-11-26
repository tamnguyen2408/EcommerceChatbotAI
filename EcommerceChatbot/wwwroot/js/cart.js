// script.js

// Lấy phần tử biểu tượng thanh toán và tab
const paymentIcon = document.getElementById('paymentIcon');
const paymentTab = document.getElementById('paymentTab');

// Thêm sự kiện khi nhấn vào biểu tượng thanh toán
paymentIcon.addEventListener('click', function () {
    // Kiểm tra xem tab có đang hiển thị không
    if (paymentTab.style.display === 'none' || paymentTab.style.display === '') {
        // Hiển thị tab
        paymentTab.style.display = 'block';
    } else {
        // Ẩn tab
        paymentTab.style.display = 'none';
    }
});
