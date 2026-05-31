🔢 Unity 2D 2048 Game
Một phiên bản trò chơi giải đố cổ điển 2048 được phát triển trên nền tảng Unity Engine 2D sử dụng hệ thống giao diện cải tiến, kiến trúc kịch bản tường minh và bộ tạo hoạt họa (Animation) mượt mà cho các khối số (Tiles).

🎮 Cơ chế trò chơi (Gameplay)
Màn chơi bao gồm một lưới ô vuông ô định. Người chơi sử dụng các phím điều hướng để di chuyển tất cả các khối số (Tiles) về một hướng (Trên, Dưới, Trái, Phải).

Logic gộp số: Khi hai khối có cùng giá trị va chạm vào nhau trong quá trình di chuyển, chúng sẽ kết hợp lại thành một khối mới có giá trị bằng tổng của hai khối đó (ví dụ: 2 + 2 = 4, 4 + 4 = 8,...).

Hệ thống tính điểm: Điểm số được cộng dồn tương ứng ngay khi người chơi gộp khối thành công.

Điều kiện thua cuộc (Game Over): Lưới ô vuông bị lấp đầy hoàn toàn bởi các khối số và không còn nước đi hợp lệ nào để gộp số.

📁 Cấu trúc thư mục mã nguồn (Assets/)
Hệ thống tài nguyên của dự án được tổ chức gọn gàng và tường minh theo tiêu chuẩn cấu trúc Unity:

Assets/
├── 📜 Scripts/                # Toàn bộ mã nguồn C# điều khiển logic trò chơi
│   ├── 📂 UI/                 # Module quản lý giao diện, bộ đếm và trạng thái game
│   │   ├── GameState.cs       # Quản lý trạng thái toàn cục (Menu, Playing, Game Over)
│   │   ├── GameTimer.cs       # Bộ đếm thời gian trôi qua trong màn chơi (Gameplay Timer)
│   │   ├── MoveCounter.cs     # Bộ đếm tổng số lần dịch chuyển các khối số của người chơi
│   │   ├── ScoreDisplay.cs    # Cập nhật và hiển thị điểm số hiện tại/điểm cao nhất lên UI
│   │   └── GameOverScene.cs   # Điều khiển màn hình kết thúc game và xử lý nút chơi lại (Restart)
│   ├── Tile.cs                # Thực thể khối số riêng lẻ (Lưu trữ giá trị, vị trí tọa độ)
│   ├── TileColor.cs           # Định nghĩa và tự động cập nhật bảng màu (Color Palette) theo giá trị số
│   ├── TileManager.cs         # Trung tâm điều phối hệ thống lưới, sinh khối số mới và xử lý di chuyển
│   ├── TileSetting.cs         # Cấu hình dữ liệu đầu vào cho các khối số
│   └── ScanMaskFollow.cs      # Script bổ trợ xử lý hiệu ứng mặt nạ quét đồ họa hiển thị
├── 📦 Prefabs/                # Các mẫu vật thể được đóng gói sẵn (Title, Tiles, UI elements)
├── 🎬 Ani/                    # Hệ thống hoạt họa (Animations) & Trình điều khiển (Controllers)
│   ├── 📂 GameOver/           # Hoạt họa chuyển cảnh màn hình kết thúc (`GameOverIn`, `GameOverOut`)
│   ├── ScoreController.controller # Trình điều khiển hoạt họa tăng điểm số
│   ├── TileControllẻ.controller   # Trình điều khiển hoạt họa cho khối số khi xuất hiện hoặc gộp
│   ├── TileAni.anim           # Hoạt họa hiệu ứng khi khối số mới sinh ra (Scale-up)
│   └── TileMerge.anim         # Hoạt họa hiệu ứng khi hai khối số gộp làm một (Pop effect)
├── 🗂️ ScriptableObject/       # Lưu trữ dữ liệu cấu hình tĩnh của hệ thống khối số
│   └── TileSetting.asset      # File cấu hình phân cấp giá trị và màu sắc hiển thị
└── 🏞️ Scenes/                 # Quản lý các phân cảnh màn chơi của dự án (`SampleScene`, `Test`)

🔧 Phân tích kỹ thuật các mã nguồn cốt lõi
1. TileManager.cs (Đầu não xử lý lưới dữ liệu)
Chịu trách nhiệm quản lý mảng 2 chiều đại diện cho lưới ô vuông trong game. Script xử lý thuật toán dịch chuyển ma trận số khi người chơi vuốt hoặc bấm phím điều hướng:

Kiểm tra các ô trống còn lại trên lưới để sinh ngẫu nhiên khối số mới (2 hoặc 4).

Kiểm tra điều kiện ngặt nghẽn để xác định trạng thái Game Over khi không còn bất kỳ ô trống nào và không có hai ô kề nhau nào cùng giá trị.

2. Tile.cs & TileColor.cs (Quản lý thuộc tính Khối số)
Tile.cs: Lưu trữ giá trị toán học hiện tại của khối (2^n) và thực hiện lệnh gọi Animator để kích hoạt các trạng thái chuyển động mượt mà từ TileControllẻ.controller.

TileColor.cs: Đọc dữ liệu từ TileSetting để thay đổi màu sắc Sprite đại diện của khối tương ứng với giá trị số tăng dần (giúp người chơi dễ dàng nhận diện cấu trúc ma trận bằng trực quan hình ảnh).

3. Module Scripts/UI/ (Hệ thống thống kê chỉ số nâng cao)
Dự án không chỉ lưu trữ điểm số cơ bản mà còn mở rộng các tính năng giúp nâng cao trải nghiệm người chơi:

GameTimer.cs: Tính toán thời gian thực chính xác giúp người chơi theo dõi tốc độ phá đảo game.

MoveCounter.cs: Ghi lại tổng số bước di chuyển, tạo thêm thử thách hoàn thành game với số bước tối ưu nhất.

🎬 Hệ thống hoạt họa mượt mà (Animation System)
Dự án tích hợp sẵn các State Machine Animator giúp trò chơi không bị thô cứng:

TileAni (Spawn Animation): Khi một khối số mới xuất hiện, nó sẽ thực hiện hiệu ứng phóng to từ tâm (Scale 0 -> 1) tránh cảm giác xuất hiện đột ngột.

TileMerge (Merge Animation): Khi hai khối gộp số, khối mới sẽ phình to nhẹ rồi thu nhỏ lại (Punch Scale) tạo phản hồi lực (Juiciness) mạnh mẽ cho người chơi.
