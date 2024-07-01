import os
extracted_text=""
def read_fifth_number(file_path):
    fifth_numbers = []
    global extracted_text
    extracted_text = read_pointnumber(file_path)
    with open(file_path, 'r') as file:
        # Skip the first 10 lines
        for _ in range(10):
            next(file)


        for line in file:
            numbers = line.split()
            if len(numbers) >= 5:
                fifth_number = int(numbers[4])
                fifth_numbers.append(fifth_number)

    return fifth_numbers
def read_pointnumber(file_path):
    with open(file_path, 'r') as file:
        lines = file.readlines()


        sixth_line = lines[5]
        extracted_text = sixth_line[-7:]
        print(f"提取的文本：{extracted_text}")
    return extracted_text
def write_to_txt(numbers_list, output_file):

    with open(output_file, 'w') as file:

        file.write("VERSION .7" + '\n')
        file.write("FIELDS label" + '\n')
        file.write("SIZE 4" + '\n')
        file.write("TYPE I" + '\n')
        file.write("COUNT 1" + '\n')
        file.write("WIDTH {}".format(extracted_text) + '\n')
        file.write("HEIGHT 1" + '\n')
        file.write("POINTS {}".format(extracted_text) + '\n')
        # file.write("VIEWPOINT 0 0 0" + '\n')
        file.write("DATA ascii" + '\n')
        for number in numbers_list:
            file.write(str(number) + '\n')

def process_files(input_folder, output_folder):
    # Ensure the output folder exists
    if not os.path.exists(output_folder):
        os.makedirs(output_folder)

    for file_name in os.listdir(input_folder):
        input_file_path = os.path.join(input_folder, file_name)
        fifth_numbers_list = read_fifth_number(input_file_path)

        output_file_path = os.path.join(output_folder, file_name)
        write_to_txt(fifth_numbers_list, output_file_path)

# Replace 'input_folder' with the path to your input folder containing the files
input_folder_path = 'D:\\XJTLU\\新建文件夹\\2023surf\cucuber\\all\\new'
# Replace 'output_folder' with the desired path for the output folder
output_folder_path = 'D:\\XJTLU\\新建文件夹\\2023surf\cucuber\\all\\label'

process_files(input_folder_path, output_folder_path)



