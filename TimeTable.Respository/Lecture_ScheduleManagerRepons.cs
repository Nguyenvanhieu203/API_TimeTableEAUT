using Dapper;
using System.Data;
using System.Data.SqlClient;
using TimeTable.DataContext.Data;
using TimeTable.DataContext.Models;
using TimeTable.Respository.Interfaces;

namespace TimeTable.Repository
{
    public class Lecture_ScheduleManagerRepons : ILecture_ScheduleManagerRepons
    {
        private readonly ConnectToSql _connectToSql;

        public Lecture_ScheduleManagerRepons(ConnectToSql connectToSql) 
        {
            _connectToSql = connectToSql;
        }
        public Task<string> AddLecture_ScheduleManagerAsync(Lecture_ScheduleManagerModel model)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteLecture_ScheduleManagerAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Lecture_ScheduleManagerModel>> GetAllLecture_ScheduleManagerAsync()
        {
            try
            {
                using (var connect = _connectToSql.CreateConnection())
                {
                    var result = await connect.QueryAsync<Lecture_ScheduleManagerModel>("GetAllLecture_ScheduleManager", commandType: CommandType.StoredProcedure);
                    return result.ToList();
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Lecture_ScheduleManagerModel> GetLecture_ScheduleManagerByNameAsync(string name)
        {
            try
            {
                using (var connect = _connectToSql.CreateConnection())
                {
                    var result = await connect.QueryFirstOrDefaultAsync<Lecture_ScheduleManagerModel>("GetAllLecture_ScheduleManagerByName", new {  Name = name }, commandType: CommandType.StoredProcedure);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<LectureSchedureMapUserModel>> SchedulingAscync(SchedulingInputModel schedulingInputModel)
        {
            try
            {
                using (var connect = _connectToSql.CreateConnection())
                {

                    int daysDifference = (int)(schedulingInputModel.DateEnd - schedulingInputModel.DateStart).TotalDays;
                    List<Class> classList = new List<Class> { };
                    List<ClassRooms> ClassRoomList = new List<ClassRooms> { };
                    List<Subject> SubjectList = new List<Subject> { };
                    foreach (var idclass in schedulingInputModel.Idclasses)
                    {
                        var classes = await connect.QueryFirstOrDefaultAsync<Class>("GetById", new { NameTable = "Class", Id = idclass }, commandType: CommandType.StoredProcedure);
                        classList.Add(classes);
                    }
                    foreach (var idclassroom in schedulingInputModel.IdclassRooms)
                    {
                        var classroom = await connect.QueryFirstOrDefaultAsync<ClassRooms>("GetById", new { NameTable = "ClassRooms", Id = idclassroom }, commandType: CommandType.StoredProcedure);
                        ClassRoomList.Add(classroom);
                    }
                    foreach (var idsubject in schedulingInputModel.Idsubjects)
                    {
                        var subject = await connect.QueryFirstOrDefaultAsync<Subject>("GetById", new { NameTable = "Subjects", Id = idsubject }, commandType: CommandType.StoredProcedure);
                        subject.appear = (subject.Credits * 5) / (daysDifference / 7);
                        SubjectList.Add(subject);

                    }
                    List<int[,]> timeTableForTotalSub = new List<int[,]>();
                    List<string[,]> timeTableForTotalClas = new List<string[,]>();
                    bool flag1 = false;
                    bool flag2 = false;
                    bool flag3 = false;
                    bool flag4 = false;

                    softTimeTable(SubjectList, timeTableForTotalSub, timeTableForTotalClas, classList, flag1, flag2, flag3, flag4, schedulingInputModel);
                    //int count = timeTableForTotalClas.Count;
                    //insertScheduleToDatabase(listIdSchedule, timeTableForTotalClas);

                }

                return new List<LectureSchedureMapUserModel>();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }


        public int[,] softFirst1(int[,] a, int classes)
        {
            int rows = 4;
            int cols = 6;
            int clas = 1;
            int[,] result = new int[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (clas == 3)
                    {
                        if (classes == 24)
                        {
                            result[i, j] = 24;
                            clas++;
                        }
                        else
                        {
                            result[i, j] = 0;
                            clas++;
                        }
                    }
                    else if (i == 2 && j == 1)
                    {
                        if (classes >= 3)
                        {
                            result[i, j] = 3;
                        }
                        else
                        {
                            result[i, j] = 0;
                        }
                    }
                    else if (clas <= classes)
                    {
                        result[i, j] = clas;
                        clas++;
                    }

                    else
                    {
                        result[i, j] = 0;
                    }
                }
            }
            a = result;
            return a;
        }
        public int[,] softFirst2(int[,] a, int classes)
        {

            int rows = 4;
            int cols = 6;
            int clas = 1;
            int[,] result = new int[rows, cols];


            for (int i = 0; i < rows; i++)
            {
                for (int j = cols - 1; j >= 0; j--)
                {
                    switch ((i, j))
                    {
                        case (1, 4):
                        case (2, 3):
                        case (3, 2):
                            result[i, j] = clas;
                            IsClassMoreThanClasses(result, classes, i, j);
                            clas++;
                            break;

                        case (2, 1):
                            result[i, j] = 4;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 0):
                            result[i, j] = 5;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 4):
                        case (3, 3):
                            result[i, j] = clas + 4;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 2):
                            result[i, j] = 8;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 1):
                            result[i, j] = 9;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 0):
                            result[i, j] = 10;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 5):
                            result[i, j] = 11;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 4):
                            result[i, j] = 12;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 3):
                            result[i, j] = 13;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 4):
                            result[i, j] = 14;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 5):
                            result[i, j] = 15;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 3):
                            result[i, j] = 16;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 2):
                            result[i, j] = 17;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 2):
                            result[i, j] = 18;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 0):
                            result[i, j] = 19;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 1):
                            result[i, j] = 20;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 5):
                            result[i, j] = 21;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 5):
                            result[i, j] = 22;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 1):
                            result[i, j] = 23;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 0):
                            result[i, j] = 24;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        default:
                            result[i, j] = 0;
                            break;
                    }
                }

            }
            return result;
        }
        public int[,] softFirst3(int[,] a, int classes, bool flag)
        {
            int rows = 4;
            int cols = 6;
            int clas = 1;
            int[,] result = new int[rows, cols];


            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    switch (i)
                    {
                        case 0:
                            switch (j)
                            {
                                case 0:
                                    result[i, j] = classes >= 16 ? 16 : 0;
                                    break;
                                case 1:
                                    result[i, j] = classes >= 17 ? 17 : 0;
                                    break;
                                case 2:
                                    result[i, j] = 1;
                                    break;
                                case 3:
                                    result[i, j] = classes >= 21 ? 21 : 0;
                                    break;
                                case 4:
                                    result[i, j] = classes >= 15 ? 15 : 0;
                                    break;
                                case 5:
                                    result[i, j] = classes >= 11 ? 11 : 0;
                                    break;
                                default:
                                    result[i, j] = 0;
                                    break;
                            }
                            break;

                        case 1:
                            switch (j)
                            {
                                case 0:
                                    result[i, j] = classes >= 4 ? 4 : 0;
                                    break;
                                case 1:
                                    result[i, j] = classes >= 18 ? 18 : 0;
                                    break;
                                case 2:
                                    result[i, j] = classes >= 19 ? 19 : 0;
                                    break;
                                case 3:
                                    result[i, j] = classes >= 20 ? 20 : 0;
                                    break;
                                case 4:
                                    result[i, j] = classes >= 12 ? 12 : 0;
                                    break;
                                case 5:
                                    result[i, j] = classes >= 3 ? 3 : 0;
                                    break;
                                default:
                                    result[i, j] = 0;
                                    break;
                            }
                            break;

                        case 2:
                            switch (j)
                            {
                                case 0:
                                    result[i, j] = classes >= 10 ? 10 : 0;
                                    break;
                                case 1:
                                    result[i, j] = classes >= 14 ? 14 : 0;
                                    break;
                                case 2:
                                    result[i, j] = classes >= 22 ? 22 : 0;
                                    break;
                                case 3:
                                    result[i, j] = classes >= 13 ? 13 : 0;
                                    break;
                                case 4:
                                    result[i, j] = classes >= 8 ? 8 : 0;
                                    break;
                                case 5:
                                    result[i, j] = classes >= 24 ? 24 : 0;
                                    break;
                                default:
                                    result[i, j] = 0;
                                    break;
                            }
                            break;

                        case 3:
                            switch (j)
                            {
                                case 0:
                                    result[i, j] = classes >= 5 ? 5 : 0;
                                    break;
                                case 1:
                                    result[i, j] = classes >= 9 ? 9 : 0;
                                    break;
                                case 2:
                                    result[i, j] = classes >= 23 ? 23 : 0;
                                    break;
                                case 3:
                                    result[i, j] = classes >= 2 ? 2 : 0;
                                    break;
                                case 4:
                                    result[i, j] = classes >= 6 ? 6 : 0;
                                    break;
                                case 5:
                                    result[i, j] = classes >= 7 ? 7 : 0;
                                    break;
                                default:
                                    result[i, j] = 0;
                                    break;
                            }
                            break;

                        default:
                            result[i, j] = 0;
                            break;
                    }

                }
            }
            flag = true;
            return result;
        }
        // Second sub
        public int[,] softSecond1(int[,] a, int classes)
        {
            int rows = 4;
            int cols = 6;
            bool hasUpdatedClas = false;
            bool hasUpdatedClas2 = false;
            int[,] result = new int[rows, cols];
            for (int i = 0; i < rows; i++)
            {

                int clas = 1;
                for (int j = 0; j < cols; j++)
                {
                    switch (i)
                    {
                        case 1:
                            if (clas == 3)
                            {
                                if (classes == 24)
                                {
                                    result[i, j] = 24;
                                    clas++;
                                }
                                else
                                {
                                    result[i, j] = 0;
                                    clas++;
                                }
                            }
                            else
                            {
                                result[i, j] = clas;
                                if (clas > classes)
                                {
                                    result[i, j] = 0;
                                }
                                clas++;
                            }
                            break;

                        case 2:
                            if (!hasUpdatedClas)
                            {
                                clas += 17;
                                hasUpdatedClas = true;
                            }
                            result[i, j] = clas;
                            if (clas > classes)
                            {
                                result[i, j] = 0;
                            }
                            clas++;
                            break;

                        case 3:
                            if (j == 1)
                            {
                                if (classes >= 3)
                                {
                                    result[i, j] = 3;
                                }
                                else
                                {
                                    result[i, j] = 0;
                                }
                            }
                            else if (classes > 12)
                            {
                                if (clas > classes)
                                {
                                    result[i, j] = 0;
                                }
                                else
                                {
                                    if (!hasUpdatedClas2)
                                    {
                                        clas += 12;
                                        hasUpdatedClas2 = true;
                                    }

                                    result[i, j] = clas;
                                    clas++;
                                }
                            }
                            break;

                        case 0:
                            if (classes > 6)
                            {
                                result[i, j] = clas + 6;
                                if ((clas + 6) > classes)
                                {
                                    result[i, j] = 0;
                                }
                                clas++;
                            }
                            else
                            {
                                result[i, j] = 0;
                            }
                            break;

                        default:
                            result[i, j] = 0;
                            break;
                    }
                }
            }
            return result;
        }
        public int[,] softSecond2(int[,] a, int classes)
        {
            int rows = 4;
            int cols = 6;
            int clas = 1;
            int[,] result = new int[rows, cols];


            for (int i = 0; i < rows; i++)
            {
                for (int j = cols - 1; j >= 0; j--)
                {
                    switch ((i, j))
                    {
                        case var _ when (i + j == 4):
                            result[i, j] = clas;
                            IsClassMoreThanClasses(result, classes, i, j);
                            clas++;
                            break;

                        case (2, 0):
                            result[i, j] = 5;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case var _ when (i == 1 && j == 4) || (i == 2 && j == 3) || (i == 3 && j == 2):
                            result[i, j] = clas + 4;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 1):
                            result[i, j] = 9;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 0):
                            result[i, j] = 10;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 5):
                            result[i, j] = 11;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 4):
                            result[i, j] = 12;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 3):
                            result[i, j] = 13;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 1):
                            result[i, j] = 14;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 5):
                            result[i, j] = 15;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 2):
                            result[i, j] = 16;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 2):
                            result[i, j] = 17;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 1):
                            result[i, j] = 18;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 0):
                            result[i, j] = 19;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 5):
                            result[i, j] = 20;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 0):
                            result[i, j] = 21;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 3):
                            result[i, j] = 22;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 4):
                            result[i, j] = 23;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 5):
                            result[i, j] = 24;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        default:
                            result[i, j] = 0;
                            break;
                    }

                }
            }
            return result;
        }
        public int[,] softSecond3(int[,] a, int classes, bool flag)
        {
            int rows = 4;
            int cols = 6;
            int clas = 1;
            int[,] result = new int[rows, cols];


            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    switch ((i, j))
                    {
                        case (0, 3):
                            result[i, j] = 1;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 2):
                            result[i, j] = 2;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 0):
                            result[i, j] = 3;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 5):
                            result[i, j] = 4;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 5):
                            result[i, j] = 5;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 1):
                            result[i, j] = 6;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 0):
                            result[i, j] = 7;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 1):
                            result[i, j] = 8;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 4):
                            result[i, j] = 9;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 5):
                            result[i, j] = 10;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 0):
                            result[i, j] = 11;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 1):
                            result[i, j] = 12;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 2):
                            result[i, j] = 13;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 4):
                            result[i, j] = 14;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 1):
                            result[i, j] = 15;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 5):
                            result[i, j] = 16;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 4):
                            result[i, j] = 17;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 4):
                            result[i, j] = 18;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 3):
                            result[i, j] = 19;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 2):
                            result[i, j] = 20;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 2):
                            result[i, j] = 21;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 3):
                            result[i, j] = 22;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 3):
                            result[i, j] = 23;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 0):
                            result[i, j] = 24;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        default:
                            result[i, j] = 0;
                            break;
                    }

                }
            }
            flag = true;
            return result;
        }
        // Third sub
        public int[,] softThird1(int[,] a, int classes)
        {
            int rows = 4;
            int cols = 6;
            int clas = 1;
            int[,] result = new int[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = cols - 1; j >= 0; j--)
                {
                    switch ((clas, i, j))
                    {
                        case (3, _, _):
                            if (classes == 24)
                            {
                                result[i, j] = 24;
                                clas++;
                            }
                            else
                            {
                                result[i, j] = 0;
                                clas++;
                            }
                            break;

                        case (_, 2, 4):
                            if (classes >= 3)
                            {
                                result[i, j] = 3;
                            }
                            else
                            {
                                result[i, j] = 0;
                            }
                            break;

                        case var _ when clas <= classes:
                            result[i, j] = clas;
                            clas++;
                            break;

                        default:
                            result[i, j] = 0;
                            break;
                    }


                }
            }
            a = result;
            return a;
        }
        public int[,] softThird2(int[,] a, int classes)
        {
            int rows = 4;
            int cols = 6;
            int clas = 1;
            int[,] result = new int[rows, cols];


            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    switch ((i, j))
                    {
                        case (1, 1):
                        case (2, 2):
                        case (3, 3):
                            result[i, j] = clas;
                            IsClassMoreThanClasses(result, classes, i, j);
                            clas++;
                            break;

                        case (2, 4):
                            result[i, j] = 4;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 5):
                            result[i, j] = 5;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 1):
                        case (3, 2):
                            result[i, j] = clas + 4;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 3):
                            result[i, j] = 8;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 4):
                            result[i, j] = 9;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 5):
                            result[i, j] = 10;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 0):
                            result[i, j] = 11;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 1):
                            result[i, j] = 12;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 2):
                            result[i, j] = 13;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 1):
                            result[i, j] = 14;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 0):
                            result[i, j] = 15;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 2):
                            result[i, j] = 16;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 3):
                            result[i, j] = 17;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 3):
                            result[i, j] = 18;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 5):
                            result[i, j] = 19;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 4):
                            result[i, j] = 20;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 5):
                            result[i, j] = 21;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 0):
                            result[i, j] = 22;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 0):
                            result[i, j] = 23;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 4):
                            result[i, j] = 24;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        default:
                            result[i, j] = 0;
                            break;
                    }

                }
            }
            return result;


        }
        public int[,] softThird3(int[,] a, int classes, bool flag)
        {
            int rows = 4;
            int cols = 6;
            int clas = 1;
            int[,] result = new int[rows, cols];


            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    switch ((i, j))
                    {
                        case (2, 3):
                            result[i, j] = 1;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 0):
                            result[i, j] = 2;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 5):
                            result[i, j] = 3;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 5):
                            result[i, j] = 4;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 2):
                            result[i, j] = 5;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 3):
                            result[i, j] = 6;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 1):
                            result[i, j] = 7;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 0):
                            result[i, j] = 8;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 0):
                            result[i, j] = 9;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 4):
                            result[i, j] = 10;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 2):
                            result[i, j] = 11;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 3):
                            result[i, j] = 12;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 1):
                            result[i, j] = 13;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 5):
                            result[i, j] = 14;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 1):
                            result[i, j] = 15;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 5):
                            result[i, j] = 16;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 4):
                            result[i, j] = 17;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 2):
                            result[i, j] = 18;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 3):
                            result[i, j] = 19;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 0):
                            result[i, j] = 20;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 1):
                            result[i, j] = 21;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 4):
                            result[i, j] = 22;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 4):
                            result[i, j] = 23;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 2):
                            result[i, j] = 24;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        default:
                            result[i, j] = 0;
                            break;
                    }

                }
            }
            flag = true;
            return result;
        }

        // Four sub
        public int[,] softFour1(int[,] a, int classes)
        {
            int rows = 4;
            int cols = 6;
            bool hasUpdatedClas = false;
            bool hasUpdatedClas2 = false;
            int[,] result = new int[rows, cols];
            for (int i = 0; i < rows; i++)
            {

                int clas = 1;
                for (int j = cols - 1; j >= 0; j--)
                {
                    switch ((clas, i, j))
                    {
                        case (3, 1, _):
                            if (classes == 24)
                            {
                                result[i, j] = 24;
                                clas++;
                            }
                            else
                            {
                                result[i, j] = 0;
                                clas++;
                            }
                            break;

                        case (_, 3, 4):
                            if (classes >= 3)
                            {
                                result[i, j] = 3;
                            }
                            else
                            {
                                result[i, j] = 0;
                            }
                            break;

                        case (int c, 0, _) when c > 6:
                            result[i, j] = clas + 6;
                            if ((clas + 6) > classes)
                            {
                                result[i, j] = 0;
                            }
                            clas++;
                            break;

                        case (int c, 1, _):
                            result[i, j] = clas;
                            if (clas > classes)
                            {
                                result[i, j] = 0;
                            }
                            clas++;
                            break;

                        case (_, 2, _):
                            if (!hasUpdatedClas)
                            {
                                clas += 17;
                                hasUpdatedClas = true;
                            }
                            result[i, j] = clas;
                            if (clas > classes)
                            {
                                result[i, j] = 0;
                            }
                            clas++;
                            break;

                        case (int c, 3, _) when c > 12:
                            if (clas > classes)
                            {
                                result[i, j] = 0;
                            }
                            else
                            {
                                if (!hasUpdatedClas2)
                                {
                                    clas += 12;
                                    hasUpdatedClas2 = true;
                                }
                                result[i, j] = clas;
                                clas++;
                            }
                            break;

                        default:
                            result[i, j] = 0;
                            break;
                    }

                }
            }
            return result;
        }

        public int[,] softFour2(int[,] a, int classes)
        {
            int rows = 4;
            int cols = 6;
            int clas = 1;
            int[,] result = new int[rows, cols];


            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    switch ((i, j))
                    {
                        case (int x, int y) when y == x + 1:
                            result[i, j] = clas;
                            IsClassMoreThanClasses(result, classes, i, j);
                            clas++;
                            break;

                        case (2, 5):
                            result[i, j] = 5;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (int a1, int b) when (a1 == 1 && b == 1) || (a1 == 2 && b == 2) || (a1 == 3 && b == 3):
                            result[i, j] = clas + 4;
                            IsClassMoreThanClasses(result, classes, i, j);
                            clas++;
                            break;

                        case (2, 4):
                            result[i, j] = 9;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 5):
                            result[i, j] = 10;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 0):
                            result[i, j] = 11;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 1):
                            result[i, j] = 12;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 2):
                            result[i, j] = 13;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 4):
                            result[i, j] = 14;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 0):
                            result[i, j] = 15;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 3):
                            result[i, j] = 16;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 3):
                            result[i, j] = 17;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 4):
                            result[i, j] = 18;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 5):
                            result[i, j] = 19;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 0):
                            result[i, j] = 20;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 0):
                            result[i, j] = 21;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 2):
                            result[i, j] = 22;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 5):
                            result[i, j] = 23;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 1):
                            result[i, j] = 24;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        default:
                            result[i, j] = 0;
                            break;
                    }

                }
            }
            return result;

        }

        public int[,] softFour3(int[,] a, int classes, bool flag)
        {
            int rows = 4;
            int cols = 6;
            int clas = 1;
            int[,] result = new int[rows, cols];


            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    switch ((i, j))
                    {
                        case (2, 2):
                            result[i, j] = 1;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 5):
                            result[i, j] = 2;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 0):
                            result[i, j] = 3;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 0):
                            result[i, j] = 4;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 3):
                            result[i, j] = 5;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 2):
                            result[i, j] = 6;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 4):
                            result[i, j] = 7;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 5):
                            result[i, j] = 8;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 5):
                            result[i, j] = 9;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 1):
                            result[i, j] = 10;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 3):
                            result[i, j] = 11;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 2):
                            result[i, j] = 12;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 4):
                            result[i, j] = 13;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 0):
                            result[i, j] = 14;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 4):
                            result[i, j] = 15;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 0):
                            result[i, j] = 16;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 1):
                            result[i, j] = 17;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (2, 3):
                            result[i, j] = 18;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 2):
                            result[i, j] = 19;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 5):
                            result[i, j] = 20;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 4):
                            result[i, j] = 21;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (0, 1):
                            result[i, j] = 22;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (1, 1):
                            result[i, j] = 23;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        case (3, 3):
                            result[i, j] = 24;
                            IsClassMoreThanClasses(result, classes, i, j);
                            break;

                        default:
                            result[i, j] = 0;
                            break;
                    }

                }
            }
            flag = true;
            return result;
        }
        public void fillSubForEachClass(int[,] timeTableForEchSub, string[,] timeTableForEachClas, Guid subID, int clas)
        {
            int rows = 4;
            int cols = 6;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (timeTableForEchSub[i, j] == clas)
                    {
                        timeTableForEachClas[i, j] = subID.ToString();
                    }
                }
            }
        }

        List<Guid> listIdSchedule = new List<Guid>();
        public async Task softTimeTable(List<Subject> listSubject, List<int[,]> timeTableForTotalSub, List<string[,]> timeTableForTotalClas, List<Class> classList, bool flag1, bool flag2, bool flag3, bool flag4, SchedulingInputModel schedulingInputModel)
        {
            int rows = 4;
            int cols = 6;
            int[,] TimeTbForEarchSub = new int[rows, cols];
            int totalClass = classList.Count;

            int cls = 0;


            foreach (var idclasses in schedulingInputModel.Idclasses)
            {
                Guid idClassRooms = schedulingInputModel.IdclassRooms[cls];
                Guid idSchedule = Guid.NewGuid();
                listIdSchedule.Add(idSchedule);
                using (var connect = _connectToSql.CreateConnection())
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT INTO Lecture_Schedules (idLecture_Schedule,idClass,idClassroom, createDate) VALUES (@id,@class,@classRoom, @createDate)";
                    cmd.Parameters.AddWithValue("@id", idSchedule);
                    cmd.Parameters.AddWithValue("@class", idclasses);
                    cmd.Parameters.AddWithValue("@classRoom", idClassRooms);
                    cmd.Parameters.AddWithValue("@createDate", DateTime.Now);
                    cmd.Connection = (SqlConnection)connect;
                    connect.Open();
                    int kq = await cmd.ExecuteNonQueryAsync();
                    int test = kq;
                }
                int i = 0;
                string[,] timeTableForEachClas = new string[rows, cols];
                foreach (var idsubject in listSubject)
                {
                    switch (i)
                    {
                        case 0:
                            switch(listSubject[i].appear)
                            {
                                case 1:
                                    TimeTbForEarchSub = softFirst1(TimeTbForEarchSub, totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    break;
                                case 2:
                                    TimeTbForEarchSub = softFirst1(TimeTbForEarchSub, totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softFirst2(TimeTbForEarchSub, totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    break;
                                case 3:
                                    TimeTbForEarchSub = softFirst1(TimeTbForEarchSub, totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softFirst2(TimeTbForEarchSub, totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softFirst3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    break;
                            }
                            break;

                        case 1:
                            switch(listSubject[i].appear)
                            {
                                case 1:
                                    TimeTbForEarchSub = softSecond1(TimeTbForEarchSub, totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    break;
                                case 2:
                                    TimeTbForEarchSub = softSecond1(TimeTbForEarchSub, totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softSecond2(TimeTbForEarchSub, totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    break;
                                case 3:
                                    TimeTbForEarchSub = softSecond1(TimeTbForEarchSub, totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softSecond2(TimeTbForEarchSub, totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softSecond3(TimeTbForEarchSub, totalClass, flag2);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    break;
                            }
                            break;

                        case 2:
                            switch (listSubject[i].appear)
                            {
                                case 1:
                                    TimeTbForEarchSub = softSecond1(TimeTbForEarchSub, totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    break;
                                case 2:
                                    TimeTbForEarchSub = softSecond1(TimeTbForEarchSub, totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softSecond2(TimeTbForEarchSub, totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    break;
                                case 3:
                                    TimeTbForEarchSub = softSecond1(TimeTbForEarchSub, totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softSecond2(TimeTbForEarchSub, totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softSecond3(TimeTbForEarchSub, totalClass, flag2);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    break;
                            }
                            break;

                        case 3:
                            switch(listSubject[i].appear)
                            {
                                case 1:
                                    TimeTbForEarchSub = softSecond1(TimeTbForEarchSub, totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    break;
                                case 2:
                                    TimeTbForEarchSub = softSecond1(TimeTbForEarchSub, totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softSecond2(TimeTbForEarchSub, totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    break;
                                case 3:
                                    TimeTbForEarchSub = softSecond1(TimeTbForEarchSub, totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softSecond2(TimeTbForEarchSub, totalClass);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softSecond3(TimeTbForEarchSub, totalClass, flag2);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    break;
                            }
                            break;

                        case 4:
                            if (listSubject[i].appear == 1)
                            {
                                if (flag1 == false)
                                {
                                    TimeTbForEarchSub = softFirst3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (flag2 == false)
                                {
                                    TimeTbForEarchSub = softSecond3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (flag3 == false)
                                {
                                    TimeTbForEarchSub = softThird3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (flag4 == false)
                                {
                                    TimeTbForEarchSub = softFour3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                            }
                            else if (listSubject[i].appear == 2)
                            {
                                if (flag1 == false && flag2 == false)
                                {
                                    TimeTbForEarchSub = softFirst3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softSecond3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (flag1 == false && flag3 == false)
                                {
                                    TimeTbForEarchSub = softFirst3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softSecond3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (flag1 == false && flag4 == false)
                                {
                                    TimeTbForEarchSub = softFirst3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softFour3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (flag2 == false && flag3 == false)
                                {
                                    TimeTbForEarchSub = softSecond3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softThird3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (flag2 == false && flag4 == false)
                                {
                                    TimeTbForEarchSub = softSecond3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softFour3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (flag3 == false && flag4 == false)
                                {
                                    TimeTbForEarchSub = softThird3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softFour3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                            }
                            else if (listSubject[i].appear == 3)
                            {
                                if (flag1 == false && flag2 == false && flag3 == false)
                                {
                                    TimeTbForEarchSub = softFirst3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softSecond3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softThird3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (flag1 == false && flag3 == false && flag4 == false)
                                {
                                    TimeTbForEarchSub = softFirst3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softThird3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softFour3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                                else if (flag2 == false && flag3 == false && flag4 == false)
                                {
                                    TimeTbForEarchSub = softSecond3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softThird3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                    TimeTbForEarchSub = softFour3(TimeTbForEarchSub, totalClass, flag1);
                                    fillSubForEachClass(TimeTbForEarchSub, timeTableForEachClas, listSubject[i].Id, cls + 1);
                                }
                            }
                            break;
                    }

                    i++;
                }
                timeTableForTotalClas.Add(timeTableForEachClas);
                cls++;
            }

            for (int ob = 0; ob < timeTableForTotalClas.Count; ob++)
            {
                string[,] currentTable = timeTableForTotalClas[ob];
                string cahoc = "";
                string ngayhoc = "";
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if (currentTable[i, j] != null)
                        {
                            switch (i)
                            {
                                case 0:
                                    cahoc = "Ca 1";
                                    break;
                                case 1:
                                    cahoc = "Ca 2";
                                    break;
                                case 2:
                                    cahoc = "Ca 3";
                                    break;
                                case 3:
                                    cahoc = "Ca 4";
                                    break;
                                case 4:
                                    cahoc = "Ca 5";
                                    break;
                            }

                            switch (j)
                            {
                                case 0:
                                    ngayhoc = "Thứ 2";
                                    break;
                                case 1:
                                    ngayhoc = "Thứ 3";
                                    break;
                                case 2:
                                    ngayhoc = "Thứ 4";
                                    break;
                                case 3:
                                    ngayhoc = "Thứ 5";
                                    break;
                                case 4:
                                    ngayhoc = "Thứ 6";
                                    break;
                                case 5:
                                    ngayhoc = "Thứ 7";
                                    break;
                            }

                            Guid idDetail = Guid.NewGuid();
                            using (var connect1 = _connectToSql.CreateConnection())
                            {
                                SqlCommand cmd1 = new SqlCommand();
                                cmd1.CommandType = CommandType.Text;
                                cmd1.CommandText = "INSERT INTO Lecture_Schedule_Detail (Id,idLecture_Schedule , idSubject , dayStudy , shiftStudy) VALUES ( @idDetail , @idLecture , @idSubject , @dayStudy , @shiftStudy )";
                                cmd1.Parameters.AddWithValue("@idDetail", idDetail);
                                cmd1.Parameters.AddWithValue("@idLecture", listIdSchedule[ob]);
                                cmd1.Parameters.AddWithValue("@idSubject", currentTable[i, j]);
                                cmd1.Parameters.AddWithValue("@dayStudy", ngayhoc);
                                cmd1.Parameters.AddWithValue("@shiftStudy", cahoc);
                                cmd1.Connection = (SqlConnection)connect1;
                                connect1.Open();
                                int kq1 = await cmd1.ExecuteNonQueryAsync();
                                int test1 = kq1;
                            }
                        }
                    }

                }

            }
        }
        public int IsClassMoreThanClasses(int[,] result, int classes, int i, int j)
        {
            if (result[i, j] > classes)
            {
                result[i, j] = 0;
            }
            return result[i, j];
        }

        public Task<string> UpdateLecture_ScheduleManagerAsync(Guid id, Lecture_ScheduleManagerModel lecture_ScheduleManagerModel)
        {
            throw new NotImplementedException();
        }
    }
}
