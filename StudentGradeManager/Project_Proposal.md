# 项目提案：学生成绩管理系统

## 项目背景

学生成绩管理是高校教务管理的核心环节。传统的 Excel 或纸质管理方式存在数据分散、易丢失、难以统计分析等问题。本系统旨在提供一款轻量级的 Windows 桌面应用，实现学生信息、课程信息、考试成绩的统一管理，并支持数据可视化与报表导出。

## 技术选型与理由

| 技术 | 选择理由 |
|------|---------|
| **C# + WinForms (.NET 8)** | 课程核心技术，快速构建桌面 GUI，原生 Windows 体验 |
| **SQLite (Microsoft.Data.Sqlite)** | 零配置嵌入式数据库，无需安装服务端，适合单机桌面应用 |
| **ClosedXML** | 开源 Excel 导出库，无需安装 Office 即可生成 .xlsx 报表 |
| **WinForms Chart** | 内置图表控件，直接可视化成绩分布 |

## 系统架构图

```
┌─────────────────────────────────────────────┐
│              MainForm (TabControl)           │
├──────────┬──────────┬──────────┬─────────────┤
│ 学生管理  │ 课程管理  │ 成绩管理  │ 统计报表    │
│ DataGrid │ DataGrid │ DataGrid │ Chart/Grid  │
├──────────┴──────────┴──────────┴─────────────┤
│              DatabaseHelper                  │
│         (CRUD + 统计查询 + 联合查询)          │
├─────────────────────────────────────────────┤
│              SQLite (grades.db)              │
└─────────────────────────────────────────────┘
```

## 功能说明

### 1. 学生管理
- 学生信息的增删改查（学号、姓名、性别、班级、入学日期）
- 按学号/姓名/班级搜索
- 删除学生时自动级联删除相关成绩记录

### 2. 课程管理
- 课程信息的增删改查（课程编号、名称、学分、授课教师）
- 删除课程时自动级联删除相关成绩记录

### 3. 成绩管理
- 成绩的录入、编辑、删除
- 选择学生和课程时使用下拉列表，避免输入错误
- 成绩范围 0-100 分

### 4. 统计报表
- 各课程平均分统计表
- 成绩分布柱状图（90-100, 80-89, 70-79, 60-69, <60）
- 支持导出全部成绩到 Excel

## 数据库设计

```sql
-- 学生表
CREATE TABLE Students (
    StudentId  TEXT PRIMARY KEY,  -- 学号
    Name       TEXT NOT NULL,     -- 姓名
    Gender     TEXT,              -- 性别
    ClassName  TEXT,              -- 班级
    EnrollmentDate TEXT           -- 入学日期
);

-- 课程表
CREATE TABLE Courses (
    CourseId TEXT PRIMARY KEY,   -- 课程编号
    Name     TEXT NOT NULL,      -- 课程名称
    Credits  REAL DEFAULT 0,    -- 学分
    Teacher  TEXT                -- 授课教师
);

-- 成绩表
CREATE TABLE Grades (
    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
    StudentId TEXT NOT NULL,     -- 学号 (FK → Students)
    CourseId  TEXT NOT NULL,     -- 课程编号 (FK → Courses)
    Score     REAL NOT NULL,     -- 成绩 (0-100)
    ExamDate  TEXT,              -- 考试日期
    FOREIGN KEY (StudentId) REFERENCES Students(StudentId),
    FOREIGN KEY (CourseId)  REFERENCES Courses(CourseId)
);
```

## 项目 Clone 地址

<!-- 作业提交时需填入实际仓库地址 -->
<!-- 例如：git@gitee.com:yourname/StudentGradeManager.git -->

> 本仓库地址：_[请在此填写你的 Gitee 仓库地址]_
