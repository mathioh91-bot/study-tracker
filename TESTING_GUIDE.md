# Study Planner - Testing & Usage Guide

## Getting Started

### Launching the Application

1. Open `WpfApp1.sln` in Visual Studio
2. Press **F5** to run (Debug mode) or **Ctrl+F5** (Release mode)
3. The application window opens with sample tasks

### Initial Screen

The application displays:
- **Left Sidebar**: Dark navy with statistics and navigation
- **Main Area**: Task list showing 5 sample tasks
- **Dashboard**: Statistics showing Total, Completed, Pending, and In Progress tasks

---

## Testing Each Feature

### 1. Adding a Task

**Steps:**
1. Click **"➕ Add Task"** button in the sidebar
2. Modal dialog opens with the title "Add New Task"
3. Enter task details:
   - **Title** *: "Physics Quiz" (required)
   - **Subject**: "Physics"
   - **Description**: "Study chapters 1-3"
   - **Deadline**: Select a date (default is tomorrow)
   - **Priority**: Select "Low", "Medium", or "High"
   - **Status**: Select "Pending", "In Progress", or "Completed"
4. Click **"Save Task"** button
5. Dialog closes, new task appears in DataGrid
6. "Total Tasks" count in sidebar increases

**Expected Results:**
- ✅ Modal dialog appears and closes properly
- ✅ New task shows in DataGrid immediately
- ✅ Statistics update automatically
- ✅ Filtered task list updates

**Edge Cases to Test:**
- Try saving without entering a title → Should not save (validation)
- Fill all fields with maximum text → Should display properly in DataGrid
- Add multiple tasks in succession → All should appear

---

### 2. Editing a Task

**Steps:**
1. Click on any task in the DataGrid to select it (row highlights)
2. Click **"✏️ Edit"** button at top right
3. Modal dialog opens with title "Edit Task"
4. Dialog shows current task values
5. Modify any field (e.g., change priority to "High")
6. Click **"Save Task"**
7. Dialog closes, DataGrid updates with changes

**Expected Results:**
- ✅ Edit button is disabled when no task selected
- ✅ Dialog pre-fills with current task values
- ✅ Changes apply immediately after save
- ✅ Filtered list updates if status/priority changed

**Test Cases:**
- Change status from "Pending" to "Completed" → Statistics update (Pending--; Completed++)
- Change deadline to past date → Should display warning (or style differently if implemented)
- Edit a task that's not visible due to filters → Still allows editing

---

### 3. Deleting a Task

**Steps:**
1. Select a task in the DataGrid (click on any row)
2. In the rightmost column, click **"🗑️ Delete"** button
3. Task disappears from DataGrid
4. Statistics update (Total tasks decreases)
5. Filtered list updates

**Expected Results:**
- ✅ Delete button appears in last DataGrid column
- ✅ Delete is instant (no confirmation dialog in current version)
- ✅ Task removed from all views
- ✅ Counts update accurately

**Test Cases:**
- Delete the last remaining task → Grid shows empty
- Delete filtered task → Only affects that row
- Delete highest priority task → Priority filter still works

---

### 4. Search Functionality

**Steps:**
1. Find the search box at top of main area
2. Type "Mathematics" in search box
3. DataGrid updates to show only matching tasks
4. Only tasks with "Mathematics" in Title or Subject appear
5. Delete search text or click clear → All tasks reappear

**Expected Results:**
- ✅ Search is case-insensitive
- ✅ Updates as you type (UpdateSourceTrigger=PropertyChanged)
- ✅ Searches in both Title and Subject fields
- ✅ Works in combination with filters

**Test Cases:**
- Search for partial word: "Math" → Finds "Mathematics"
- Search for subject: "Physics" → Finds physics tasks
- Search with filters active → Combines both criteria
- Search with no results → Grid shows empty
- Clear search → All tasks reappear

---

### 5. Status Filtering

**Steps:**
1. Locate the filter buttons in the control bar
2. Find "All Status | Pending | In Progress | Completed"
3. Click "Pending" button
4. DataGrid shows only pending tasks
5. Statistics in sidebar reflect filtered counts (if implemented)
6. Click "All Status" to show all

**Expected Results:**
- ✅ Buttons toggle filter on/off
- ✅ Only selected status tasks appear
- ✅ Can filter by one status at a time
- ✅ Works with search and priority filters

**Test Cases:**
- Click "Pending" → Shows pending tasks
- Click "Completed" → Shows completed tasks (should be empty initially)
- Complete a task → It disappears from Pending, appears if filtering by Completed
- Apply search + status filter → Shows intersection of both

---

### 6. Priority Filtering

**Steps:**
1. Find priority filter buttons: "All Priority | Low | Medium | High"
2. Click "High" button
3. DataGrid shows only high-priority tasks
4. Click other buttons to filter by different priorities
5. Combine with status filter for more specific results
6. Click "All Priority" to reset

**Expected Results:**
- ✅ Filters display only matching priority tasks
- ✅ Multiple tasks can have same priority
- ✅ Combines well with status and search filters
- ✅ Can toggle filter on/off by clicking same button

**Test Cases:**
- Filter by "High" priority → Shows 2 sample tasks
- Filter by "Low" priority → Shows 1 sample task
- Filter "High" priority + "Completed" status → Shows intersection
- Search + Priority filter → All three filters work together

---

### 7. Multi-Filter Combination

**Steps:**
1. Enter search text: "Math"
2. Click status filter: "In Progress"
3. Click priority filter: "High"
4. DataGrid shows only tasks matching ALL criteria:
   - Search text contains "Math"
   - Status is "In Progress"
   - Priority is "High"
5. Results update dynamically as you adjust filters

**Expected Results:**
- ✅ All three filters apply together (AND logic, not OR)
- ✅ Updating any filter immediately refreshes results
- ✅ Clear Filters button resets all three

**Test Cases:**
- Apply restrictive filters → May show 0-1 tasks
- Search + Status → Shows subset
- Add Priority filter → Further restricts
- Change any criteria → Results update instantly

---

### 8. Clear Filters & Search

**Steps:**
1. Apply search, status, and priority filters
2. DataGrid shows restricted results
3. Click **"🔄 Clear Filters"** button in sidebar
4. All filters clear
5. Search box empties
6. Full task list reappears

**Expected Results:**
- ✅ All filters reset
- ✅ Search text clears
- ✅ Full unfiltered list appears
- ✅ Statistics show total counts

---

### 9. Dashboard Statistics

**Sidebar Statistics Cards:**
- **Total Tasks**: Count of all tasks
- **✓ Completed**: Count of completed tasks (green)
- **⏳ Pending**: Count of pending tasks (orange)
- **⚡ In Progress**: Count of in-progress tasks (blue)

**Testing:**
1. Note initial statistics (5 tasks total)
2. Add a task → "Total Tasks" increases to 6
3. Edit task status to "Completed" → Pending decreases, Completed increases
4. Delete a completed task → Total and Completed decrease
5. Filter by status → Filtered counts still show correctly

**Expected Results:**
- ✅ Statistics update in real-time
- ✅ Always sum correctly
- ✅ Reflected immediately on task changes
- ✅ Color-coded for visual clarity

---

### 10. Modal Dialog Interactions

**Opening Dialog:**
1. Click "Add Task" or Edit button
2. Modal appears with semi-transparent overlay
3. Background becomes slightly darkened
4. Cannot interact with main UI until dialog closes

**Closing Dialog:**
1. Click "Cancel" → Dialog closes, no changes saved
2. Click "Save Task" → Saves data and closes
3. Click outside dialog (if clickable) → May close (depends on implementation)

**Form Validation:**
1. Try saving task with blank title
2. Should either show error or prevent save

**Expected Results:**
- ✅ Modal properly blocks interaction with background
- ✅ Cancel correctly discards changes
- ✅ Save validates and applies changes
- ✅ Dialog closes properly both ways

---

### 11. Visual Design Elements

**Color Scheme:**
- **Sidebar**: Dark navy (#001F3F) with white text
- **Buttons**: Blue accent (#00A4EF) for primary, white for secondary
- **Cards**: White background with subtle shadows
- **Icons**: Unicode emoji for visual appeal (📚, ✓, ⏳, ⚡, ➕, ✏️, 🗑️)

**Hover Effects:**
1. Hover over buttons → Color changes slightly
2. Hover over DataGrid rows → Subtle background color change
3. Click button → Pressing animation

**Spacing and Layout:**
- Consistent margins and padding
- Responsive grid layout
- Professional typography

---

## Sample Data Reference

The application loads with 5 sample tasks:

| ID | Title | Subject | Deadline | Priority | Status |
|---|---|---|---|---|---|
| 1 | Study Mathematics Chapter 5 | Mathematics | +2 days | High | In Progress |
| 2 | Read Literature Book Chapter 3 | Literature | +5 days | Medium | Pending |
| 3 | Physics Lab Report | Physics | +1 day | High | In Progress |
| 4 | History Essay | History | +7 days | Medium | Pending |
| 5 | Chemistry Quiz Preparation | Chemistry | +3 days | Low | Pending |

**Use these for testing filters and search!**

---

## Performance Testing

### Large Dataset (if modified to support more tasks)
- **100 tasks**: Should display and filter smoothly
- **1000 tasks**: Virtualization keeps performance good
- **Rapid additions**: Should not lag or stutter

### Memory Usage
- Each task object: ~200 bytes
- 1000 tasks: ~200 KB (negligible)
- Application baseline: ~50-100 MB (typical WPF app)

### Responsiveness
- Click to action: < 50 ms
- Filter application: < 100 ms
- Large collection update: < 200 ms

---

## Keyboard Navigation

| Key | Action |
|---|---|
| Tab | Move between controls |
| Enter | Click selected button / Accept dialog |
| Escape | Close modal dialog |
| Arrow keys | Navigate DataGrid rows |
| Ctrl+F | Focus search box (if implemented) |

---

## Troubleshooting Guide

### Issue: "Task appears to not save"
**Solution:** Check that Title field is not empty (required field)

### Issue: "DataGrid shows no rows"
**Solution:** 
- Click "Clear Filters" to reset all filters
- Check that filters aren't too restrictive

### Issue: "Statistics show incorrect counts"
**Solution:** 
- Filter all tasks by "All Status" and "All Priority"
- Verify counts match DataGrid rows

### Issue: "Modal dialog won't open"
**Solution:** 
- Ensure ViewModel is properly initialized
- Check that DataContext is set in MainWindow.xaml.cs

### Issue: "Binding errors in output window"
**Solution:**
- Check property names for typos
- Verify DataContext is MainViewModel instance
- Enable binding error logging

### Issue: "Search not working"
**Solution:**
- Check that UpdateSourceTrigger=PropertyChanged is set
- Verify SearchText property exists on ViewModel
- Type slowly to confirm updates occur

---

## Test Checklist (Complete)

### Basic CRUD Operations
- [ ] Add Task: Successfully creates and displays
- [ ] Edit Task: Modifies existing task
- [ ] Delete Task: Removes from grid
- [ ] Cancel Edit: Discards changes

### Search & Filter
- [ ] Search by title: "Study Mathematics"
- [ ] Search by subject: "Physics"
- [ ] Filter by status: "Pending", "In Progress", "Completed"
- [ ] Filter by priority: "Low", "Medium", "High"
- [ ] Multi-filter: Search + Status + Priority
- [ ] Clear filters: Resets all to defaults

### Data Integrity
- [ ] Add task → Appears in grid immediately
- [ ] Edit task → Changes visible after save
- [ ] Delete task → Removed from all views
- [ ] Statistics → Update with each change

### UI/UX
- [ ] Modal dialog opens/closes smoothly
- [ ] Buttons have hover effects
- [ ] Cards have shadows and rounded corners
- [ ] Color scheme is consistent
- [ ] Icons display correctly
- [ ] Text is readable and properly aligned

### Edge Cases
- [ ] Empty title: Cannot save
- [ ] Very long text: Displays without overflow
- [ ] No tasks after filtering: Grid shows empty
- [ ] Rapid additions: No crashes or hangs
- [ ] Select then deselect task: Edit button disabled

### Performance
- [ ] Application starts quickly
- [ ] Filtering is instant
- [ ] Adding tasks is smooth
- [ ] No memory leaks over time

---

## Success Criteria

✅ Application successfully builds
✅ All sample tasks load on startup
✅ Add/Edit/Delete operations work correctly
✅ Search filters tasks by title and subject
✅ Status and priority filters work independently
✅ Multi-filter combinations work together
✅ Statistics update automatically
✅ UI is modern and professional
✅ No crashes or errors occur
✅ Response times are fast

---

If all tests pass, the **Study Planner** application is working correctly! 🎉

