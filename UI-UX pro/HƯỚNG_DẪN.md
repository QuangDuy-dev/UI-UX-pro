# 🦐 UIAnimate Gallery — Hướng dẫn & Bộ Prompt

App ASP.NET Core MVC + MongoDB Atlas tự động sinh & lưu các component animation HTML/CSS/JS,
kèm cron job hàng ngày tìm UI/UX trends trên web và tự thêm animation mới.

## 1. Chạy app

```powershell
cd "D:\UI-UX pro\UI-UX pro"
dotnet run --launch-profile http
```

- Web: http://localhost:5165
- API: http://localhost:5165/api/animations
- Upload (công khai): http://localhost:5165/Upload
- Admin (route bí mật): http://localhost:5165/Admin-Secret-Manager-Key-999
  → đổi route tại `appsettings.json → AdminSettings:SecretRoute`
  → mật khẩu đăng nhập tại `AdminSettings:MasterPassword` (mặc định `uianimate-admin-2026`)

Lần chạy đầu: tự seed 23 danh mục + 26 template animation vào MongoDB Atlas (db `uianimationdb`).

## 2. Cấu trúc project

```
Controllers/   Home (gallery/search/detail), Upload (form), Admin (route bí mật + login + settings), Api (JSON cho agent)
Models/        AnimationItem, Category, TrendReport, Enums, AdminViewModels
Services/      AnimationService (CRUD/search), CategoryService,
               TemplateLibrary (26 template animation), TrendService (sinh từ keywords + LLM opt-in),
               AdminConfigService (đọc/ghi admin-config.json), AdminAuthService (cookie HMAC),
               PreviewBuilder (ghép HTML/CSS/JS → srcdoc an toàn)
Filters/       AdminAuthorizeAttribute (chặn truy cập admin nếu không có cookie hợp lệ)
Data/          MongoDbContext (MongoDB Atlas)
Views/         Index (gallery + search + danh mục), Detail (preview + copy code), Upload,
               Admin (Index dashboard, Login, Settings & Automation, Trends)
wwwroot/       css/site.css, js/site.js
appsettings.json  ← connection string MongoDB + AdminSettings (route/mật khẩu) + Ai (mặc định)
admin-config.json ← (tự sinh) cấu hình AI/automation do user đổi ở trang Settings & Automation
```

## 3. Bảo mật Admin (Cookie-based, không cần bảng Account trong DB)

Khu vực Admin nằm sau route bí mật. Ai truy cập chưa đăng nhập sẽ bị chuyển về trang login
nhỏ chỉ gồm 1 ô mật khẩu. Đăng nhập đúng → cấp cookie ký HMAC (HttpOnly, hết hạn 12h).

- **Đổi route bí mật**: `appsettings.json → AdminSettings:SecretRoute` (đang là `Admin-Secret-Manager-Key-999`)
- **Đổi mật khẩu**: `appsettings.json → AdminSettings:MasterPassword`
- **Gallery `/` và Upload `/Upload` vẫn mở công khai** — chỉ admin bị chặn.

> 💡 Không tạo bảng Account trong MongoDB — tiết kiệm tài nguyên, chỉ dùng 1 mật khẩu master + cookie.

## 4. Bật/tắt LLM & chỉnh tốc độ sinh (Settings & Automation)

Vào **Admin (route bí mật) → "Settings & Automation"**:

- **Bật tự động (Auto-update)**: bật/tắt cron logic (lịch cron vẫn chạy ở OpenSquilla, nhưng app đọc
  trạng thái này để quyết định có sinh hay không).
- **Dùng LLM (AiEnabled)**: bật để gọi API LLM sinh code sáng tạo; tắt để dùng template library (miễn phí).
- **Provider / Model / API Key / Endpoint**: đổi trực tiếp, lưu vào file `admin-config.json` (ngoài source, an toàn).
- **MaxItemsPerRun**: số animation mỗi lần chạy (1–20).
- **Trend Keywords**: danh sách keyword mặc định.
- **⚡ Run Trend Job Now**: bấm để chạy ngay lập tức cả pipeline (quét trend → gọi LLM/template →
  đẩy vào MongoDB) mà không cần chờ cron. Kết quả hiện ngay trên gallery `/`.

Cấu hình được lưu bằng file `admin-config.json` (tự sinh trong thư mục project), được nạp khi app
khởi động và mỗi lần chạy job — đổi xong bấm **Run Trend Job Now** là áp dụng ngay, không cần build lại.

> Nếu bật LLM mà chưa nhập API Key → **tự fallback sang template** (không lỗi, không tốn phí).
> Nếu LLM gọi lỗi (401 key sai / endpoint lệch provider, 429, network...) → **cũng tự fallback template**,
> job vẫn tạo animation, report có `status="partial"` + `error` mô tả rõ lỗi LLM.
>
> ⚠️ **Lỗi "LLM failed: ... 401 (Unauthorized)" nghĩa là gì?** Đó là API key/endpoint/model bị lệch
> (VD: endpoint OpenAI nhưng model DeepSeek, hoặc key không hợp lệ cho provider đang chọn).
> Cách sửa: vào Settings → chọn đúng **Provider** (OpenAI/DeepSeek/...) — endpoint sẽ tự đổi theo
> provider (hoặc nhập tay), sau đó nhập đúng API key & model của provider đó → Lưu → Run Trend Job Now.

### Bảo mật API cho Agent (`ApiSecurity`)

- `appsettings.json → ApiSecurity:AgentKey` để **trống** (mặc định): API `/api/*` **công khai**,
  cron agent gọi tự do không bị chặn — khuyên dùng cho OpenSquilla cron (tool `http_request` chặn
  header dạng bí mật nên agent không gửi được X-Api-Key).
- Điền giá trị (VD `"my-secret-key"`): mọi request tới `/api/*` **bắt buộc header `X-Api-Key`** khớp,
  sai/thiếu trả 401. Dùng khi muốn chặn người ngoài gọi API (VD app deploy public):
  ```bash
  curl -X POST http://localhost:5165/api/daily-trends/run -H "Content-Type: application/json" -H "X-Api-Key: my-secret-key" -d '{"keywords":["glassmorphism"],"maxItems":3}'
  ```
- **Admin UI không bị ảnh hưởng**: các trang `/Admin-Secret-Manager-Key-999/*` vẫn dùng cookie
  (`AdminAuthorize`), hoàn toàn tách biệt với cơ chế X-Api-Key của API.
- Job ID: `1e440474-d965-4e3b-a8de-2da13b5ffdbb`
- Cơ chế: agent (OpenSquilla) dùng `web_search` tìm UI/UX trends → chọn 3–5 keywords →
  gọi `POST http://localhost:5165/api/daily-trends/run` → app sinh & lưu animation + trend report.
- Xem lịch sử chạy: trang `/Admin/Trends` hoặc `GET /api/daily-trends/reports`.

> ⚠️ Cron chỉ hoạt động khi app đang chạy (endpoint localhost). Để app luôn chạy,
> mở thêm 1 terminal chạy `dotnet run` hoặc cài Windows Task Scheduler (xem mục 6).

## 6. API dành cho agent

| Method | Endpoint | Mô tả |
|---|---|---|
| GET | `/api/animations?category=&q=&sort=&page=` | Danh sách công khai |
| POST | `/api/animations` | Upload 1 item bất kỳ (JSON) |
| POST | `/api/daily-trends/run` | ⭐ Sinh animation mới từ keywords + ghi report |
| GET | `/api/daily-trends/reports` | Lịch sử các lần chạy daily |
| POST | `/api/animations/{id}/view` · `/like` | Tăng view/like |

Ví dụ gọi daily-trends:

```bash
curl -X POST http://localhost:5165/api/daily-trends/run \
  -H "Content-Type: application/json" \
  -d '{"keywords":["glassmorphism","bento grid","scroll reveal"],"summary":"...","sourceUrls":["https://..."],"maxItems":5}'
```

## 7. (Tuỳ chọn) Tự động chạy app mỗi ngày bằng Windows Task Scheduler

1. Mở Task Scheduler → Create Basic Task → tên `UIAnimateApp`.
2. Trigger: Daily, 07:50 (trước cron 08:00).
3. Action: Start a program
   - Program: `powershell.exe`
   - Arguments: `-NoProfile -Command "Set-Location 'D:\UI-UX pro\UI-UX pro'; dotnet run --launch-profile http"`
4. Bật "Run whether user is logged on or not".

## 8. Bộ prompt (nếu muốn tái tạo ở agent khác / nơi khác)

### Prompt gốc xây dựng app (đã thực hiện)

> "Xây web app ASP.NET Core MVC (.NET 10) kết nối MongoDB Atlas (connection string trong
> appsettings.json, db uianimationdb). Tính năng: (1) gallery hiển thị các component
> animation HTML/CSS/JS dạng card preview iframe srcdoc sandbox, lọc theo danh mục
> (nav, hero, button, card, list, table, form, loader, modal, toast, tabs, accordion,
> badge, progress, carousel, marquee, counter, chat, footer, dropdown, pricing, scroll),
> search theo tên/tag/mô tả, sort (newest/popular/liked/name), phân trang;
> (2) trang Upload nhập tên, danh mục, tags, HTML/CSS/JS kèm live preview cập nhật theo input
> rồi lưu vào MongoDB; (3) trang Detail preview lớn + tabs code + nút copy từng phần/copy tất cả
> + like/view count + related; (4) Admin quản lý item (publish/delete), nút chạy generate daily
> thủ công, xem trend reports; (5) REST API: GET/POST /api/animations,
> POST /api/daily-trends/run (nhận keywords, summary, sourceUrls, maxItems → sinh & lưu items +
> tạo TrendReport), GET /api/daily-trends/reports, POST view/like;
> (6) TrendService: template-based (26 template animation chất lượng trong TemplateLibrary,
> map keywords→category, biến thể màu palette theo trend, dedupe tên) + opt-in LLM
> (Ai:Enabled/ApiKey/Endpoint/Model trong appsettings → gọi chat completions, parse JSON,
> fallback template khi thiếu key); (7) seed tự động categories + templates khi DB trống;
> (8) dark theme đẹp, responsive. Bảo mật: user code chạy trong iframe sandbox allow-scripts."

### Prompt cron job hàng ngày (đang chạy trong OpenSquilla cron, job `78768ebd`)

> "Mỗi ngày 08:00 (Asia/Bangkok): dùng web_search tìm UI/UX design trends hiện tại
> (Awwwards, Dribbble, Smashing Magazine...), chọn 3–5 keywords nổi bật, viết summary ngắn tiếng Việt,
> gọi POST http://localhost:5165/api/daily-trends/run với JSON
> {keywords, summary, sourceUrls, maxItems:5} và header Content-Type: application/json, sau đó báo cáo
> bằng tiếng Việt: số animation mới tạo, keywords đã dùng, và lưu ý nếu status=error/empty.
> App đọc cấu hình AI (bật/tắt, API key, model, maxItems) từ admin-config.json do user chỉnh ở
> trang Admin → Settings & Automation; việc chọn template hay gọi LLM do app tự quyết định.
> Nếu app chưa chạy thì báo rõ: app chưa chạy, hãy chạy dotnet run, không bịa số liệu."

### Prompt để agent khác (Claude/Codex/DeepSeek) xây tương tự

> "Bạn là senior full-stack .NET developer. Tạo web app 'UIAnimate Gallery': thư viện component
> animation HTML/CSS/JS có thể copy-paste, tự động cập nhật mỗi ngày từ UI/UX trends.
> Stack: ASP.NET Core MVC (.NET 10) + MongoDB Atlas (MongoDB.Driver) + Razor + vanilla JS.
> Yêu cầu: gallery + search + danh mục + phân trang; trang upload HTML/CSS/JS với live preview
> sandbox; trang chi tiết với tabs code + copy; admin; REST API cho agent gọi
> (POST /api/daily-trends/run nhận keywords → sinh animation mới từ thư viện template +
> lưu TrendReport); background job hoặc cron ngoài gọi API mỗi ngày. Seed ít nhất 20 template
> animation chất lượng. Giao diện dark theme hiện đại. Viết code hoàn chỉnh, build sạch,
> chạy được ngay."

### Prompt nâng cấp 2: bảo mật Admin bằng route bí mật + cookie (không dùng DB Account)

> "Nâng cấp app ASP.NET Core MVC hiện có, KHÔNG tạo bảng Account trong database để tiết kiệm tài nguyên.
> (1) Đổi route Admin từ /Admin thành route bí mật khó đoán đọc từ appsettings.json →
> AdminSettings:SecretRoute (vd 'Admin-Secret-Manager-Key-999'). Ai truy cập route bí mật mà chưa có
> cookie xác thực hợp lệ → chuyển hướng về trang đăng nhập nhỏ chỉ 1 ô mật khẩu (hoặc trả 403 cho AJAX).
> Đăng nhập đúng AdminSettings:MasterPassword → cấp cookie ký bằng HMAC (HttpOnly, hết hạn 12h, SameSite=Lax).
> Gallery '/' và Upload '/Upload' vẫn mở công khai hoàn toàn. Dùng IAuthorizationFilter tùy chỉnh
> (AdminAuthorizeAttribute) áp lên controller; action Login đánh dấu [AllowAnonymous] để filter bỏ qua;
> Logout xoá cookie.
> (2) Thêm cấu hình AI vào appsettings.json (Ai:Enabled, ApiKey, Endpoint, Model, MaxItemsPerRun) và quản lý
> bằng file JSON local 'admin-config.json' (tự sinh, đọc/ghi bằng AdminConfigService singleton) để bật/tắt
> auto-update, đổi API key, model, max items mà không cần build lại. Tại trang Admin tạo tab
> 'Settings & Automation' để bật/tắt Auto/LLM, sửa API key + model + endpoint + MaxItems + trend keywords,
> và nút 'Run Trend Job Now' gọi POST admin/run-trend-job chạy ngay pipeline (quét trend → gọi LLM → sinh
> code HTML/CSS/JS → đẩy MongoDB) không cần chờ cron. Sau khi user đổi setting, app đọc admin-config.json
> làm nguồn sự thật; nếu bật LLM mà thiếu API key thì tự fallback template (không lỗi). App vẫn giữ cơ chế
> tự động nạp dữ liệu trực tiếp vào collection 'animations' để gallery công khai thấy ngay khi agent hoàn tất.
> Viết code đầy đủ, build sạch, chạy được."
